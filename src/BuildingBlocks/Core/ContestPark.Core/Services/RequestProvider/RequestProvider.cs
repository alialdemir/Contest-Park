using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ContestPark.Core.Services.RequestProvider
{
    public class RequestProvider : IRequestProvider
    {
        #region Private variable

        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger<RequestProvider> _logger;

        #endregion Private variable

        #region Constructor

        public RequestProvider(ILogger<RequestProvider> logger)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        public Task<TResult> GetAsync<TResult>(string url)
        {
            return SendAsync<TResult>(HttpMethod.Get, url);
        }

        public Task<TResult> PostAsync<TResult>(string url, object data = null, string authorization = "")
        {
            return SendAsync<TResult>(HttpMethod.Post, url, data);
        }

        private HttpClient CreateHttpClient(string authorization)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(authorization))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorization);
            }

            return httpClient;
        }

        private async Task<TResult> SendAsync<TResult>(HttpMethod httpMethod, string url, object data = null, string authorization = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authorization);
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, url);

                if (data != null)
                {
                    string content = await Task.Run(() => JsonConvert.SerializeObject(data));
                    httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string serialized = await response.Content.ReadAsStringAsync();

                    TResult result = await Task.Run(() =>
                        JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Request isteği atılırken hata oluştu Http Method: {httpMethod.Method} Url{url}", ex);
            }

            return default;
        }

        #endregion Methods
    }
}
