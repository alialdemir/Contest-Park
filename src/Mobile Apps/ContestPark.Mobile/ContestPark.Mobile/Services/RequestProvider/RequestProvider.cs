using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Models.ErrorModel;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Polly.Wrap;
using Prism.Ioc;
using Prism.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Services.RequestProvider
{
    public class RequestProvider : IRequestProvider
    {
        #region Private variable

        private readonly IAnalyticsService _analyticsService;
        private readonly ISettingsService _settingsService;
        private readonly ConcurrentDictionary<string, AsyncPolicyWrap> _policyWrappers;

        private readonly JsonSerializerSettings _serializerSettings;

        #endregion Private variable

        #region Constructor

        public RequestProvider(IAnalyticsService analyticsService,
                               ISettingsService settingsService)
        {
            _analyticsService = analyticsService;
            _settingsService = settingsService;
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
        public Task<ResponseModel<TResult>> DeleteAsync<TResult>(string url, object data = null)
        {
            return SendAsync<TResult>(HttpMethod.Delete, url, data);
        }

        /// <summary>
        /// Servisten get işlemi yapar
        /// </summary>
        /// <typeparam name="TResult">İstenilen tip de değeri döndürür</typeparam>
        /// <param name="url">Servis get Url</param>
        /// <returns>TResult tipinde veri</returns>
        public Task<ResponseModel<TResult>> GetAsync<TResult>(string url)
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
        public Task<ResponseModel<TResult>> PostAsync<TResult>(string url, object data = null)
        {
            return SendAsync<TResult>(HttpMethod.Post, url, data);
        }

        /// <summary> Login process </summary> <typeparam name="TResult"></typeparam> <param
        /// name="url"></param> <param name="clientId"></param> <param name="scopes"></param> <param
        /// name="loginModel"></param> <></returns>
        public async Task<ResponseModel<TResult>> PostAsync<TResult>(string url, Dictionary<string, string> dictionary)
        {
            HttpClient httpClient = CreateHttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            try
            {
                // a new StringContent must be created for each retry as it is disposed after each call
                //var origin = GetOriginFromUri(url);

                //return HttpInvoker(origin, async (context) =>
                //{
                if (await CheckNetworkAsync())
                    return new ResponseModel<TResult>();

                HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(dictionary)
                });

                return await ResultRequest<TResult>(response);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

                return new ResponseModel<TResult>();
            }
        }

        private async Task<ResponseModel<TResult>> ResultRequest<TResult>(HttpResponseMessage response)
        {
            //await HandleResponse(response);

            ResponseModel<TResult> result = new ResponseModel<TResult>();

            string serialized = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode && serialized.Contains("invalid_username_or_password"))
            {
                result.Error = new ValidationResultModel
                {
                    ErrorMessage = "invalid_username_or_password"
                };
            }
            if (!response.IsSuccessStatusCode && response.RequestMessage.RequestUri.AbsoluteUri.Contains("/api/v1/Account") && (serialized.Contains("userName") || serialized.Contains("fullName")))
            {
                // TODO: burada üye olma validasyonunda standarta uygun gelmeli
                result.Error = new ValidationResultModel
                {
                    MemberNames = JsonConvert.DeserializeObject<SignUpValidationModel>(serialized, _serializerSettings).Errors
                };
            }
            else if (!response.IsSuccessStatusCode)
            {
                result.Error = JsonConvert.DeserializeObject<ValidationResultModel>(serialized, _serializerSettings);
            }
            else if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(serialized))
            {
                result.Data = JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings);
            }

            result.IsSuccess = response.IsSuccessStatusCode;

            result.HttpStatusCode = response.StatusCode;

            return result;
        }

        /// <summary>
        /// Dosya upload işlemi yapar
        /// </summary>
        /// <typeparam name="TResult">İstenilen tip de değeri döndürür</typeparam>
        /// <param name="url">Servis post Url</param>
        /// <param name="file">Yüklenecek dosya</param>
        /// <returns>TResult tipinde veri</returns>
        public Task<ResponseModel<TResult>> PostAsync<TResult>(string url, Stream file)
        {
            if (file == null)
                return Task.FromResult(new ResponseModel<TResult>());

            return SendAsync<TResult>(HttpMethod.Post, url, file);
        }

        #endregion Methods

        #region Private methods

        private HttpClient CreateHttpClient()
        {
            //   ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //   ServicePointManager.ServerCertificateValidationCallback +=
            //(sender, cert, chain, error) =>
            //{
            //    return true;
            //};

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (_settingsService != null)
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(_settingsService.CurrentUser.Language.ToLanguageCode()));

                string token = _settingsService.AuthAccessToken;
                if (!string.IsNullOrEmpty(token))
                {
                    string tokenType = _settingsService.TokenType ?? "Bearer";
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
                }
            }

            return httpClient;
        }

        private async Task<ResponseModel<TResult>> SendAsync<TResult>(HttpMethod httpMethod, string url, object data = null)
        {
            using (HttpClient httpClient = CreateHttpClient())
            {
                try
                {
                    if (await CheckNetworkAsync())
                        return new ResponseModel<TResult>();

                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                    if (data != null)
                    {
                        if (data.GetType() == typeof(MediaModel))
                        {
                            httpClient.Timeout = TimeSpan.FromMinutes(1);
                        }

                        httpRequestMessage.Content = GetContent(data);
                    }

                    HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);

                    return await ResultRequest<TResult>(response);
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);

                    _analyticsService.SendEvent("Hata", "Servis Hatalari", ex.Message);

                    return new ResponseModel<TResult>();
                }
            }
        }

        private async Task<bool> CheckNetworkAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IPageDialogService pageDialogService = RegisterTypesConfig.Container.Resolve<IPageDialogService>();

                await pageDialogService?.DisplayAlertAsync(ContestParkResources.NoInternet, "", ContestParkResources.Okay);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Http content oluşturur
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Http content</returns>
        private HttpContent GetContent(object data)
        {
            if (data == null)
                return null;

            if (data.GetType() == typeof(MediaModel))
            {
                MediaModel media = (MediaModel)data;
                if (media == null || media.File == null || string.IsNullOrEmpty(media.FileName))
                    return null;

                StreamContent streamContent = new StreamContent(media.File);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(GetImageContentType(media.FileName));

                return new MultipartFormDataContent
                        {
                            { streamContent, "files", media.FileName }
                        };
            }
            else if (data != null)
            {
                string content = JsonConvert.SerializeObject(data);
                return new StringContent(content, Encoding.UTF8, "application/json");
            }

            return null;
        }

        /// <summary>
        /// Resim adına göre content type döndürür
        /// </summary>
        /// <param name="path">Resim adı</param>
        /// <returns>Content type</returns>
        private string GetImageContentType(string path)
        {
            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".jpg": return "image/jpeg";
                case ".png": return "image/png";
            }

            return string.Empty;
        }

        #endregion Private methods
    }
}
