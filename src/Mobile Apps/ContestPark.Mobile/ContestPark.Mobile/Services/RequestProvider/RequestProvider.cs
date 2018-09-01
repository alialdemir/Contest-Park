﻿using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Exceptions;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Services.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Wrap;
using Prism.Ioc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.RequestProvider
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when invoking HTTP services.
    /// Based on Polly library: https://github.com/App-vNext/Polly
    /// </summary>
    public class RequestProvider : IRequestProvider
    {
        #region Private variable

        private readonly Func<string, IEnumerable<Policy>> _policyCreator;

        private readonly ConcurrentDictionary<string, PolicyWrap> _policyWrappers;

        private readonly JsonSerializerSettings _serializerSettings;

        #endregion Private variable

        #region Constructor

        public RequestProvider(Func<string, IEnumerable<Policy>> policyCreator)
        {
            _policyCreator = policyCreator;
            _policyWrappers = new ConcurrentDictionary<string, PolicyWrap>();

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
        /// Login process
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url"></param>
        /// <param name="clientId"></param>
        /// <param name="scopes"></param>
        /// <param name="loginModel"></param>
        /// <></returns>
        public async Task<TResult> PostAsync<TResult>(string url, string clientId, string scopes, LoginModel loginModel)
        {
            // a new StringContent must be created for each retry
            // as it is disposed after each call
            //    var origin = GetOriginFromUri(url);
            //return HttpInvoker(origin, async () =>
            //{
            HttpClient httpClient = CreateHttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var from = new Dictionary<string, string>
                {
                    {"username",loginModel.UserName },
                    {"password",loginModel.Password },
                    {"client_id",clientId },
                    {"grant_type","password" },
                    {"scope",scopes },
                };
            var response = await httpClient.PostAsync(
                url,
                new FormUrlEncodedContent(from));

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            TResult result = await Task.Run(() =>
                JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

            return result;
            //});
        }

        #endregion Methods

        #region Private methods

        private Task<TResult> SendAsync<TResult>(HttpMethod httpMethod, string url, object data = null)
        {
            // a new StringContent must be created for each retry
            // as it is disposed after each call
            var origin = GetOriginFromUri(url);

            return HttpInvoker(origin, async () =>
            {
                HttpClient httpClient = CreateHttpClient();
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, url);

                if (data != null)
                {
                    string content = await Task.Run(() => JsonConvert.SerializeObject(data));
                    httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
                await HandleResponse(response);

                string serialized = await response.Content.ReadAsStringAsync();

                TResult result = await Task.Run(() =>
                    JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

                return result;
            });
        }

        private async Task<T> HttpInvoker<T>(string origin, Func<Task<T>> action)
        {
            var normalizedOrigin = NormalizeOrigin(origin);

            if (!_policyWrappers.TryGetValue(normalizedOrigin, out PolicyWrap policyWrap))
            {
                policyWrap = Policy.WrapAsync(_policyCreator(normalizedOrigin).ToArray());
                _policyWrappers.TryAdd(normalizedOrigin, policyWrap);
            }

            // Executes the action applying all
            // the policies defined in the wrapper
            return await policyWrap.ExecuteAsync(action, new Context(normalizedOrigin));
        }

        private static string NormalizeOrigin(string origin)
        {
            return origin?.Trim().ToLower();
        }

        private static string GetOriginFromUri(string uri)
        {
            var url = new Uri(uri);

            var origin = $"{url.Scheme}://{url.DnsSafeHost}:{url.Port}";

            return origin;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(/*new NativeMessageHandler()*/);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();
            if (settingsService != null)
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(settingsService.Language.ToLanguageCode()));

                string token = settingsService.AuthAccessToken;
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return httpClient;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }

        #endregion Private methods
    }
}