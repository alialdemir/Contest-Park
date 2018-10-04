using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.User;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Settings
{
    public class SettingsMockService : ISettingsService
    {
        #region Setting Constants

        private const string signalRConnectionIdDefault = "SignalRConnectionIdDefault";

        private readonly string AccessTokenDefault = string.Empty;

        private readonly string IdTokenDefault = string.Empty;

        private readonly bool IsPrivatePriceDefault = false;

        private readonly bool IsSoundEffectActiveDefaultDefault = true;

        private readonly Languages LanguageDefault = Languages.English;

        private readonly string SignalRConnectionIdDefault = string.Empty;

        #endregion Setting Constants

        #region Settings Properties

        public string AuthAccessToken
        {
            get => GetValueOrDefault(AccessTokenDefault);
            set => AddOrUpdateValue(value);
        }

        public Languages Language
        {
            get => (Languages)GetValueOrDefault((byte)LanguageDefault);
            set => AddOrUpdateValue((byte)value);
        }

        public string SignalRConnectionId { get; set; }

        private UserInfoModel _userInfo;

        public UserInfoModel CurrentUser
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = new UserInfoModel
                    {
                        CoverPicturePath = DefaultImages.DefaultCoverPicture,
                        FullName = "Ali Aldemir",
                        Language = Languages.Turkish,
                        ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                        UserId = "1111-1111-1111-1111",
                        UserName = "witcherfearless"
                    };
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

        #endregion Settings Properties

        #region Public Methods

        public Task AddOrUpdateValue(bool value, [CallerMemberName]string methodName = "") => AddOrUpdateValueInternal(methodName, value);

        public Task AddOrUpdateValue(string value, [CallerMemberName]string methodName = "") => AddOrUpdateValueInternal(methodName, value);

        public Task AddOrUpdateValue(byte value, [CallerMemberName]string methodName = "") => AddOrUpdateValueInternal(methodName, value);

        public bool GetValueOrDefault(bool defaultValue, [CallerMemberName] string methodName = "") => GetValueOrDefaultInternal(methodName, defaultValue);

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
                Debug.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
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
                Debug.WriteLine("Unable to remove: " + key, " Message: " + ex.Message);
            }
        }

        public Task SetSettings(SettingTypes settingType, string settingValue)
        {
            return Task.CompletedTask;
        }

        #endregion Internal Implementation
    }
}