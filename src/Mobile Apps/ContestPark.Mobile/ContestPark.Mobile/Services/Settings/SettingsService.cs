using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.RequestProvider;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Setting Constants

        private const string signalRConnectionIdDefault = "SignalRConnectionIdDefault";

        private readonly string AccessTokenDefault = string.Empty;

        private readonly string IdTokenDefault = string.Empty;
        private readonly bool IsPrivatePriceDefault = false;
        private readonly bool IsSoundEffectActiveDefaultDefault = true;
        private readonly string RefleshTokenDefault = string.Empty;

        private readonly string SignalRConnectionIdDefault = string.Empty;
        private readonly string TokenTypeDefault = "Bearer";

        #endregion Setting Constants

        #region Private variables

        private const string ApiUrlBase = "api/v1/settings";
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public SettingsService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        #endregion Constructor

        #region Settings Properties

        private UserInfoModel _userInfo;

        public string AuthAccessToken
        {
            get => GetValueOrDefault(AccessTokenDefault);
            set => AddOrUpdateValue(value);
        }

        public UserInfoModel CurrentUser
        {
            get
            {
                if (_userInfo == null)
                {
                    RefreshCurrentUser();
                }

                return _userInfo;
            }
        }

        public bool IsPrivatePrice
        {
            get => GetValueOrDefault(IsPrivatePriceDefault);
            set => AddOrUpdateValue(value);
        }

        public bool IsSoundEffectActive
        {
            get => GetValueOrDefault(IsSoundEffectActiveDefaultDefault);
            set => AddOrUpdateValue(value);
        }

        public string RefreshToken
        {
            get => GetValueOrDefault(RefleshTokenDefault);
            set => AddOrUpdateValue(value);
        }

        public string SignalRConnectionId { get; set; }

        public string TokenType
        {
            get => GetValueOrDefault(TokenTypeDefault);
            set => AddOrUpdateValue(value);
        }

        #endregion Settings Properties

        #region Public Methods

        public Task AddOrUpdateValue(bool value, [CallerMemberName]string methodName = "") => AddOrUpdateValueInternal(methodName, value);

        public Task AddOrUpdateValue(string value, [CallerMemberName]string methodName = "") => AddOrUpdateValueInternal(methodName, value);

        public Task AddOrUpdateValue(byte value, [CallerMemberName]string methodName = "") => AddOrUpdateValueInternal(methodName, value);

        public bool GetValueOrDefault(bool defaultValue, [CallerMemberName]string methodName = "") => GetValueOrDefaultInternal(methodName, defaultValue);

        public string GetValueOrDefault(string defaultValue, [CallerMemberName]string methodName = "") => GetValueOrDefaultInternal(methodName, defaultValue);

        public int GetValueOrDefault(byte defaultValue, [CallerMemberName]string methodName = "") => GetValueOrDefaultInternal(methodName, defaultValue);

        #endregion Public Methods

        #region Internal Implementation

        private async Task AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (value == null)
            {
                await Remove(key);
            }

            Application.Current.Properties[key] = value;
            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
            }
        }

        private T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            object value = null;
            if (Application.Current.Properties.ContainsKey(key))
            {
                value = Application.Current.Properties[key];
            }
            return null != value ? (T)value : defaultValue;
        }

        private async Task Remove(string key)
        {
            try
            {
                if (Application.Current.Properties[key] != null)
                {
                    Application.Current.Properties.Remove(key);
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to remove: " + key, " Message: " + ex.Message);
            }
        }

        #endregion Internal Implementation

        #region CurrentUser methods

        /// <summary>
        /// Current user refresh
        /// </summary>
        public void RefreshCurrentUser()
        {
            _userInfo = _userInfo.GetUserInfo(AuthAccessToken);
        }

        public void RemoveCurrentUser()
        {
            _userInfo = new UserInfoModel();
        }

        #endregion CurrentUser methods

        #region Setting Service

        /// <summary>
        /// Ayar tipine göre değerini kayıt eder
        /// </summary>
        /// <param name="settingType">Ayar tipi</param>
        /// <param name="settingValue">Değeri</param>
        public async Task SetSettingsAsync(SettingTypes settingType, string settingValue)// TODO: parametreden gönderirke problem olabilir property name eklenmesi lazım
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}{(byte)settingType}", settingValue);

            await _requestProvider.PostAsync<string>(uri);
        }

        #endregion Setting Service
    }
}