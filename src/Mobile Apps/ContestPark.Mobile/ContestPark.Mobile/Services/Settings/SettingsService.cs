using Com.OneSignal;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Models.Duel.Bet;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Setting Constants

        private readonly bool IsTutorialDisplayedDefault = false;
        private readonly string AccessTokenDefault = string.Empty;
        private readonly string CurrentUserDefault = string.Empty;

        private readonly bool IsSoundEffectActiveDefault = true;

        private readonly string RefleshTokenDefault = string.Empty;

        private readonly string TokenTypeDefault = "Bearer";
        private readonly byte SignUpCountDefault = 0;

        private readonly string LastUpdatedScopeNameDefault = string.Empty;
        private readonly string LastSelectedBetDefault = string.Empty;
        private readonly List<int> PendingDuelIdsDefault = new List<int>();

        #endregion Setting Constants

        #region Settings Properties

        private ISettings AppSettings => CrossSettings.Current;

        /// <summary>
        /// Tamamlanmamış düello id ekle
        /// </summary>
        /// <param name="duelId">Düello id</param>
        public void AddPendingDuelId(int duelId)
        {
            if (!PendingDuelIdsDefault.Any(x => x == duelId))
            {
                PendingDuelIdsDefault.Add(duelId);

                AppSettings.AddOrUpdateValue(nameof(PendingDuelIdsDefault), PendingDuelIdsDefault);
            }
        }

        /// <summary>
        /// Tamamlanmamış bekleyen düello id varsa onu siler
        /// </summary>
        /// <param name="duelId">Düello id</param>
        public void RemovePendingDuelId(int duelId)
        {
            if (PendingDuelIdsDefault.Any(x => x == duelId))
            {
                PendingDuelIdsDefault.Remove(duelId);

                AppSettings.AddOrUpdateValue(nameof(PendingDuelIdsDefault), PendingDuelIdsDefault);
            }
        }

        /// <summary>
        /// Tamamlanmamış düello id'leri
        /// </summary>
        /// <returns>Tamamlanmayan düello id'leri</returns>
        public List<int> GetPendingDuelIds()
        {
            return AppSettings.GetValueOrDefault(nameof(PendingDuelIdsDefault), PendingDuelIdsDefault);
        }

        private UserInfoModel _userInfo;

        public string AuthAccessToken
        {
            get => SecureStorage.GetAsync(nameof(AuthAccessToken)).Result;
            set => SecureStorage.SetAsync(nameof(AuthAccessToken), value);
        }

        /// <summary>
        /// Üye olma sayısı
        /// </summary>
        public byte SignUpCount
        {
            get => (byte)AppSettings.GetValueOrDefault(nameof(SignUpCount), SignUpCountDefault);
            set => AppSettings.AddOrUpdateValue(nameof(SignUpCount), value);
        }

        public UserInfoModel CurrentUser
        {
            get
            {
                if (_userInfo == null)
                {
                    string currentUserJson = AppSettings.GetValueOrDefault(nameof(CurrentUser), CurrentUserDefault);
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
            get => AppSettings.GetValueOrDefault(nameof(IsSoundEffectActive), IsSoundEffectActiveDefault);
            set => AppSettings.AddOrUpdateValue(nameof(IsSoundEffectActive), value);
        }

        public bool IsTutorialDisplayed
        {
            get => AppSettings.GetValueOrDefault(nameof(IsTutorialDisplayed), IsTutorialDisplayedDefault);
            set => AppSettings.AddOrUpdateValue(nameof(IsTutorialDisplayed), value);
        }

        public string RefreshToken
        {
            get => AppSettings.GetValueOrDefault(nameof(RefreshToken), RefleshTokenDefault);
            set => AppSettings.AddOrUpdateValue(nameof(RefreshToken), value);
        }

        public string SignalRConnectionId { get; set; }

        public string TokenType
        {
            get => AppSettings.GetValueOrDefault(nameof(TokenType), TokenTypeDefault);
            set => AppSettings.AddOrUpdateValue(nameof(TokenType), value);
        }

        public string LastUpdatedScopeName
        {
            get => AppSettings.GetValueOrDefault(nameof(LastUpdatedScopeName), LastUpdatedScopeNameDefault);
            set => AppSettings.AddOrUpdateValue(nameof(LastUpdatedScopeName), value);
        }

        private BetModel _lastSelectedBet;

        public BetModel LastSelectedBet
        {
            get
            {
                if (_lastSelectedBet == null)
                {
                    string jsonLastSelectedBalanceType = AppSettings.GetValueOrDefault(nameof(LastSelectedBet), LastSelectedBetDefault);
                    if (!string.IsNullOrEmpty(jsonLastSelectedBalanceType))
                        _lastSelectedBet = JsonConvert.DeserializeObject<BetModel>(jsonLastSelectedBalanceType);
                }

                return _lastSelectedBet;
            }

            set
            {
                _lastSelectedBet = value;

                AppSettings.AddOrUpdateValue(nameof(LastSelectedBet), value);
            }
        }

        #endregion Settings Properties

        #region CurrentUser methods

        /// <summary>
        /// Current user refresh
        /// </summary>
        public void RefreshCurrentUser(UserInfoModel currentUser)
        {
            _userInfo = currentUser;

            TranslateExtension.CultureInfo = null;// i18n tekrar culture yüklemesi için null a çektik

            var culture = new CultureInfo(currentUser.Language.ToLanguageCode());

            ContestParkResources.Culture = culture;

            DependencyService.Get<ILocalize>().SetCultureInfo(culture);

            OneSignal.Current.SendTag("UserId", currentUser.UserId);

            AppSettings.AddOrUpdateValue(nameof(CurrentUser), currentUser);
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

        #endregion Setting Service
    }

    public static class CrossSettingsExtension
    {
        public static bool AddOrUpdateValue<T>(this ISettings settings, string key, T value, string fileName = null)
        {
            if (value == null || value.GetType() != typeof(object))
                return false;

            string json = JsonConvert.SerializeObject(value);

            return settings.AddOrUpdateValue(key, json, fileName);
        }

        public static T GetValueOrDefault<T>(this ISettings settings, string key, T defaultValue, string fileName = null)
        {
            string json = settings.GetValueOrDefault(key, string.Empty, fileName);

            if (string.IsNullOrEmpty(json))
                return defaultValue;

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
