using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.RequestProvider;
using Newtonsoft.Json;
using System;
using System.Globalization;
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
        private readonly string CurrentUserDefault = string.Empty;

        private readonly string IdTokenDefault = string.Empty;
        private readonly bool IsSoundEffectActiveDefaultDefault = true;
        private readonly string RefleshTokenDefault = string.Empty;

        private readonly string SignalRConnectionIdDefault = string.Empty;
        private readonly string TokenTypeDefault = "Bearer";
        private readonly byte SignUpCountDefault = 0;

        #endregion Setting Constants

        #region Private variables

        private const string ApiUrlBase = "api/v1/account";
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

        /// <summary>
        /// Üye olma sayısı
        /// </summary>
        public byte SignUpCount
        {
            get => GetValueOrDefault(SignUpCountDefault);
            set => AddOrUpdateValue(value);
        }

        public UserInfoModel CurrentUser
        {
            get
            {
                if (_userInfo == null)
                {
                    string currentUserJson = GetValueOrDefault(CurrentUserDefault);
                    if (!string.IsNullOrEmpty(currentUserJson))
                    {
                        _userInfo = JsonConvert.DeserializeObject<UserInfoModel>(currentUserJson);
                    }
                    else
                    {
                        CultureInfo cultureInfo = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                        _userInfo = new UserInfoModel()

                        {
                            Language = cultureInfo.IetfLanguageTag.ToLanguagesEnum()
                        };
                    }
                }

                return _userInfo;
            }
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

        public byte GetValueOrDefault(byte defaultValue, [CallerMemberName]string methodName = "") => GetValueOrDefaultInternal(methodName, defaultValue);

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
        public void RefreshCurrentUser(UserInfoModel currentUser)
        {
            string currentUserJson = JsonConvert.SerializeObject(currentUser);

            _userInfo = currentUser;

            TranslateExtension.CultureInfo = null;// i18n tekrar culture yüklemesi için null a çektik

            var culture = new CultureInfo(currentUser.Language.ToLanguageCode());

            ContestParkResources.Culture = culture;

            DependencyService.Get<ILocalize>().SetCultureInfo(culture);

            AddOrUpdateValue(currentUserJson, nameof(CurrentUser));
        }

        public void RemoveCurrentUser()
        {
            _userInfo = new UserInfoModel();
        }

        #endregion CurrentUser methods

        #region Setting Service

        /// <summary>
        /// Login olunca set edilmesi gereken değerleri set eder
        /// </summary>
        /// <param name="userToken">Token info</param>
        public void SetTokenInfo(UserToken userToken)
        {
            AuthAccessToken = userToken.AccessToken;
            RefreshToken = userToken.RefreshToken;
            TokenType = userToken.TokenType;
        }

        /// <summary>
        /// Şuanki kullanıcı bilgileri
        /// </summary>
        /// <returns>Login olan kullanıcı bilgileri</returns>
        public async Task<UserInfoModel> GetUserInfo()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/UserInfo");

            var result = await _requestProvider.GetAsync<UserInfoModel>(uri);

            return result.Data;
        }

        #endregion Setting Service
    }
}
