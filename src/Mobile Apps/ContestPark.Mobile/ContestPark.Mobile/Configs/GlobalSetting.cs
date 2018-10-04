using System;

namespace ContestPark.Mobile.Configs
{
    public class GlobalSetting
    {
        public const string DefaultEndpoint = "http://169.254.80.80:5106";

        private string _baseIdentityEndpoint;
        private string _baseGatewayShoppingEndpoint;

        public GlobalSetting()
        {
            BaseIdentityEndpoint = DefaultEndpoint;
            BaseGatewayCategoryEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting();

        public string BaseIdentityEndpoint
        {
            get { return _baseIdentityEndpoint; }
            set
            {
                _baseIdentityEndpoint = value;
                UpdateEndpoint(_baseIdentityEndpoint);
            }
        }

        public string BaseGatewayCategoryEndpoint
        {
            get { return _baseGatewayShoppingEndpoint; }
            set
            {
                _baseGatewayShoppingEndpoint = value;
                UpdateGatewayCategoryEndpoint(_baseGatewayShoppingEndpoint);
            }
        }

        public string ClientId => "xamarin";

        public string Scope { get; set; } = "category cp duel signalrhub question";

        public string RegisterWebsite { get; set; }

        public string AuthorizeEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string LogoutCallback { get; set; }

        public string GatewaEndpoint { get; set; }

        public TimeSpan CacheExpireIn { get; } = TimeSpan.FromMinutes(60);

        public string SignalREndpoint { get; set; }

        public bool IsMockData { get; set; }

        private void UpdateEndpoint(string endpoint)
        {
            RegisterWebsite = $"{endpoint}/Account/Register";
            LogoutCallback = $"{endpoint}/Account/Redirecting";

            var connectBaseEndpoint = $"{endpoint}/connect";
            AuthorizeEndpoint = $"{connectBaseEndpoint}/authorize";
            UserInfoEndpoint = $"{connectBaseEndpoint}/userinfo";
            TokenEndpoint = $"{connectBaseEndpoint}/token";
            LogoutEndpoint = $"{connectBaseEndpoint}/endsession";

            SignalREndpoint = $"{endpoint.Replace(":5106", "")}:5105/contestpark";
        }

        private void UpdateGatewayCategoryEndpoint(string endpoint)
        {
            GatewaEndpoint = $"{endpoint}";
        }
    }
}