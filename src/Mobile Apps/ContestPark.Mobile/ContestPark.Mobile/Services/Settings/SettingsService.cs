using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.User;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Setting Constants

        private const string AccessToken = "access_token";
        private const string language = "languages";
        private const string signalRConnectionIdDefault = "SignalRConnectionIdDefault";
        private readonly string AccessTokenDefault = string.Empty;
        private readonly string IdTokenDefault = string.Empty;
        private readonly Languages LanguageDefault = Languages.English;
        private readonly string SignalRConnectionIdDefault = string.Empty;

        #endregion Setting Constants

        #region Settings Properties

        public string AuthAccessToken
        {
            get => GetValueOrDefault(AccessToken, AccessTokenDefault);
            set => AddOrUpdateValue(AccessToken, value);
        }

        public Languages Language
        {
            get => (Languages)GetValueOrDefault(language, (byte)LanguageDefault);
            set => AddOrUpdateValue(language, (byte)value);
        }

        public string SignalRConnectionId { get; set; }
        //public string SignalRConnectionId
        //{
        //    get => GetValueOrDefault(signalRConnectionIdDefault, SignalRConnectionIdDefault);
        //    set => AddOrUpdateValue(signalRConnectionIdDefault, value);
        //}

        private UserInfoModel _userInfo;

        public UserInfoModel UserInfo
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = JwtTokenDecoder.GetUserInfo(AuthAccessToken);
                }

                return _userInfo;
            }
        }

        #endregion Settings Properties

        #region Public Methods

        public Task AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);

        public Task AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);

        public Task AddOrUpdateValue(string key, byte value) => AddOrUpdateValueInternal(key, value);

        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public int GetValueOrDefault(string key, byte defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

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
    }
}