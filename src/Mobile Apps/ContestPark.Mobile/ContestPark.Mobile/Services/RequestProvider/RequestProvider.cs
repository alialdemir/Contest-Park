using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Exceptions;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Services.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Wrap;
using Prism.Ioc;
using Prism.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Services.RequestProvider
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit breaker policies when invoking HTTP
    /// services. Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class RequestProvider : IRequestProvider
    {
        #region Private variable

        private readonly Func<string, IEnumerable<AsyncPolicy>> _policyCreator;

        private readonly ConcurrentDictionary<string, AsyncPolicyWrap> _policyWrappers;

        private readonly JsonSerializerSettings _serializerSettings;

        #endregion Private variable

        #region Constructor

        public RequestProvider(Func<string, IEnumerable<AsyncPolicy>> policyCreator)
        {
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, AsyncPolicyWrap>();

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Servisten delete işlemi yapar
        /// </summary>
        /// <typeparam name="TResult">İstenilen tip de değeri döndürür</typeparam>
        /// <param name="url">Servis delete Url</param>
        /// <param name="data">Content data</param>
        /// <returns>TResult tipinde veri</returns>
        public Task<TResult> DeleteAsync<TResult>(string url, object data = null)
        {
            return SendAsync<TResult>(HttpMethod.Delete, url, data);
        }

        /// <summary>
        /// Servisten get işlemi yapar
        /// </summary>
        /// <typeparam name="TResult">İstenilen tip de değeri döndürür</typeparam>
        /// <param name="url">Servis get Url</param>
        /// <returns>TResult tipinde veri</returns>
        public Task<TResult> GetAsync<TResult>(string url)
        {
            return SendAsync<TResult>(HttpMethod.Get, url);
        }

        /// <summary>
        /// Servisten post işlemi yapar
        /// </summary>
        /// <typeparam name="TResult">İstenilen tip de değeri döndürür</typeparam>
        /// <param name="url">Servis post Url</param>
        /// <param name="data">Content data</param>
        /// <returns>TResult tipinde veri</returns>
        public Task<TResult> PostAsync<TResult>(string url, object data = null)
        {
            return SendAsync<TResult>(HttpMethod.Post, url, data);
        }

        /// <summary> Login process </summary> <typeparam name="TResult"></typeparam> <param
        /// name="url"></param> <param name="clientId"></param> <param name="scopes"></param> <param
        /// name="loginModel"></param> <></returns>
        public async Task<TResult> PostAsync<TResult>(string url, Dictionary<string, string> dictionary)
        {
            // a new StringContent must be created for each retry
            // as it is disposed after each call
            //    var origin = GetOriginFromUri(url);
            //return HttpInvoker(origin, async () =>
            //{
            HttpClient httpClient = CreateHttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            var response = await httpClient.PostAsync(
                url,
                new FormUrlEncodedContent(dictionary));

            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
            //});
        }

        /// <summary>
        /// Dosya upload işlemi yapar
        /// </summary>
        /// <typeparam name="TResult">İstenilen tip de değeri döndürür</typeparam>
        /// <param name="url">Servis post Url</param>
        /// <param name="file">Yüklenecek dosya</param>
        /// <returns>TResult tipinde veri</returns>
        public Task<TResult> PostAsync<TResult>(string url, Stream file)
        {
            if (file == null)
                return Task.FromResult(default(TResult));

            return SendAsync<TResult>(HttpMethod.Post, url, file);
        }

        #endregion Methods

        #region Private methods

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim().ToLower();
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();
            if (settingsService != null)
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(settingsService.CurrentUser.Language.ToLanguageCode()));

                string token = settingsService.AuthAccessToken;
                if (!string.IsNullOrEmpty(token))
                {
                    string tokenType = settingsService.TokenType ?? "Bearer";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }
            }

            return httpClient;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Context, Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out AsyncPolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }

        private async Task test()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string content = JsonConvert.SerializeObject(new
            {
                Email = "test12@test.com",
                FullName = "sadss",
                LanguageCode = "tr-TR",
                Password = "19931993",
                UserName = "UserName"
            });

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://192.168.1.177:5105/api/v1/account");

            httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);

                string serialized = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async Task<TResult> SendAsync<TResult>(HttpMethod httpMethod, string url, object data = null)
        {
            await test();
            // a new StringContent must be created for each retry as it is disposed after each call
            var origin = GetOriginFromUri(url);

            //  return HttpInvoker<TResult>(origin, async (context) =>
            //{
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IPageDialogService pageDialogService = RegisterTypesConfig.Container.Resolve<IPageDialogService>();

                await pageDialogService?.DisplayAlertAsync(ContestParkResources.NoInternet, "", ContestParkResources.Okay);

                return default;
            }

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, url);

            if (data.GetType() == typeof(FileStream))
            {
                if (data == null)
                    return default(TResult);

                httpRequestMessage.Content = new MultipartFormDataContent
                        {
                            { new StreamContent((Stream)data), "file", "filename" }// TODO: filename kısmında uzantı isteyebilir
                        };
            }
            else if (data != null)
            {
                string content = JsonConvert.SerializeObject(data);//  await Task.Factory.StartNew(() => ();
                httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            HttpClient httpClient = CreateHttpClient();

            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            await HandleResponse(response);

            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
            //    });
        }

        #endregion Private methods
    }
}
