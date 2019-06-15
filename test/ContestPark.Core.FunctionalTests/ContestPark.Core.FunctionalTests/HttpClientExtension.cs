using System.Net.Http;

namespace ContestPark.Core.FunctionalTests
{
    public static class HttpClientExtension
    {
        public static HttpClient AddLangCode(this HttpClient httpClient, string langCode)
        {
            if (!string.IsNullOrEmpty(langCode))
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(langCode));
            }

            return httpClient;
        }
    }
}