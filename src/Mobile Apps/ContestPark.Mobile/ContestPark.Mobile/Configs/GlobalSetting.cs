﻿using System;

namespace ContestPark.Mobile.Configs
{
    /// <summary>
    /// Defines the <see cref="GlobalSetting"/>
    /// </summary>
    public class GlobalSetting
    {
        #region Constants

        /// <summary>
        /// Defines the DefaultEndpoint
        /// </summary>
        public const string DefaultEndpoint = "http://172.17.98.65:5105";

        #endregion Constants

        #region Fields

        /// <summary>
        /// Defines the _baseGatewayShoppingEndpoint
        /// </summary>
        private string _baseGatewayShoppingEndpoint;

        /// <summary>
        /// Defines the _baseIdentityEndpoint
        /// </summary>
        private string _baseIdentityEndpoint;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSetting"/> class.
        /// </summary>
        public GlobalSetting()
        {
            BaseIdentityEndpoint = DefaultEndpoint;
            BaseGatewayCategoryEndpoint = DefaultEndpoint;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the Instance
        /// </summary>
        public static GlobalSetting Instance { get; } = new GlobalSetting();

        /// <summary>
        /// Gets or sets the AuthorizeEndpoint
        /// </summary>
        public string AuthorizeEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the BaseGatewayCategoryEndpoint
        /// </summary>
        public string BaseGatewayCategoryEndpoint
        {
            get { return _baseGatewayShoppingEndpoint; }
            set
            {
                _baseGatewayShoppingEndpoint = value;
                UpdateGatewayCategoryEndpoint(_baseGatewayShoppingEndpoint);
            }
        }

        /// <summary>
        /// Gets or sets the BaseIdentityEndpoint
        /// </summary>
        public string BaseIdentityEndpoint
        {
            get { return _baseIdentityEndpoint; }
            set
            {
                _baseIdentityEndpoint = value;
                UpdateEndpoint(_baseIdentityEndpoint);
            }
        }

        /// <summary>
        /// Gets the CacheExpireIn
        /// </summary>
        public TimeSpan CacheExpireIn { get; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Gets the ClientId
        /// </summary>
        public string ClientId => "xamarin";

        /// <summary>
        /// Gets or sets the GatewaEndpoint
        /// </summary>
        public string GatewaEndpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsMockData
        /// </summary>
        public bool IsMockData { get; set; }

        /// <summary>
        /// Gets or sets the LogoutCallback
        /// </summary>
        public string LogoutCallback { get; set; }

        /// <summary>
        /// Gets or sets the LogoutEndpoint
        /// </summary>
        public string LogoutEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the RegisterWebsite
        /// </summary>
        public string RegisterWebsite { get; set; }

        /// <summary>
        /// Gets or sets the Scope
        /// </summary>
        public string Scope { get; set; } = "category identity balance chat duel follow signalr post question offline_access";

        /// <summary>
        /// Gets or sets the SignalREndpoint
        /// </summary>
        public string SignalREndpoint { get; set; }

        /// <summary>
        /// Gets or sets the TokenEndpoint
        /// </summary>
        public string TokenEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the UserInfoEndpoint
        /// </summary>
        public string UserInfoEndpoint { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The UpdateEndpoint
        /// </summary>
        /// <param name="endpoint">The endpoint <see cref="string"/></param>
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

        /// <summary>
        /// The UpdateGatewayCategoryEndpoint
        /// </summary>
        /// <param name="endpoint">The endpoint <see cref="string"/></param>
        private void UpdateGatewayCategoryEndpoint(string endpoint)
        {
            GatewaEndpoint = $"{endpoint}";
        }

        #endregion Methods
    }
}
