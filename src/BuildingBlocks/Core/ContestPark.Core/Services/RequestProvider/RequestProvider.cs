using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ContestPark.Core.Services.HttpService
{
    public class RequestProvider : IRequestProvider
    {
        #region Private variable

        private readonly JsonSerializerSettings _serializerSettings;

        private IConfiguration Configuration { get; }

        #endregion Private variable

        #region Constructor

        public RequestProvider(IConfiguration configuration)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
            Configuration = configuration;
        }

        #endregion Constructor

        #region Methods

        public Task<TResult> GetAsync<TResult>(string url)
        {
            return SendAsync<TResult>(HttpMethod.Get, url);
        }

        public Task<TResult> PostAsync<TResult>(string url, object data = null)
        {
            return SendAsync<TResult>(HttpMethod.Post, url, data);
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string serviceName = Configuration["Audience"];
            string clientKey = Configuration["ClientKey"];
            if (!string.IsNullOrEmpty(serviceName) && !string.IsNullOrEmpty(clientKey))// servisler arası istek gittiğini anlaması için bunları ekledik
            {
                httpClient.DefaultRequestHeaders.Add("ServiceName", serviceName);
                httpClient.DefaultRequestHeaders.Add("ClientKey", clientKey);
            }

            return httpClient;
        }

        private async Task<TResult> SendAsync<TResult>(HttpMethod httpMethod, string url, object data = null)
        {
            try
            {
                HttpClient httpClient = CreateHttpClient();
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
                Debug.WriteLine(ex);
            }

            return default(TResult);
        }

        #endregion Methods
    }
}