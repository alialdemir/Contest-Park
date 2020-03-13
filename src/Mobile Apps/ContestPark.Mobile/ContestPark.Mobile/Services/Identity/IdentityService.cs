using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.ErrorModel;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Base;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/Account";
        private readonly IPageDialogService _dialogService;
        private readonly ICacheService _cacheService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IRequestProvider _requestProvider;
        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISignalRServiceBase _signalRServiceBase;

        #endregion Private variables

        #region Constructor

        public IdentityService(
            IRequestProvider requestProvider,
            IPageDialogService dialogService,
            ICacheService cacheService,
            IAnalyticsService analyticsService,
            ISettingsService settingsService,
            IEventAggregator eventAggregator,
            ISignalRServiceBase signalRServiceBase)
        {
            _requestProvider = requestProvider;
            _dialogService = dialogService;
            _cacheService = cacheService;
            _analyticsService = analyticsService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
            _signalRServiceBase = signalRServiceBase;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Rastgele bot kullanıcı bilgileri verir
        /// </summary>
        /// <returns>Kullanıcı bilgileri</returns>
        public async Task<RandomUserModel> GetRandomBotUser()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/RandomUser");
            var response = await _requestProvider.GetAsync<RandomUserModel>(uri);
            if (!response.IsSuccess)
                return null;

            return response.Data;
        }

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        /// <param name="picture">Resim stream</param>
        public async Task ChangeCoverPictureAsync(MediaModel media)
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ChangeCoverPicture");

            var response = await _requestProvider.PostAsync<PictureModel>(uri, media);
            if (response.Error != null)
                await ShowValidationMessages(new string[1] { response.Error.ErrorMessage });

            if (response.IsSuccess)
            {
                _eventAggregator
                           .GetEvent<ChangeUserInfoEvent>()
                           .Publish(new UserInfoModel
                           {
                               CoverPicturePath = response.Data.PicturePath
                           });

                _eventAggregator
                           .GetEvent<PostRefreshEvent>()
                           .Publish();
            }

            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        /// <param name="picture">Resim stream</param>
        public async Task ChangeProfilePictureAsync(MediaModel media)
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ChangeProfilePicture");

            var response = await _requestProvider.PostAsync<PictureModel>(uri, media);
            if (response.Error != null)
                await ShowValidationMessages(new string[1] { response.Error.ErrorMessage });

            if (response.IsSuccess)
            {
                _eventAggregator
                           .GetEvent<ChangeUserInfoEvent>()
                           .Publish(new UserInfoModel
                           {
                               ProfilePicturePath = response.Data.PicturePath
                           });

                _eventAggregator
                           .GetEvent<PostRefreshEvent>()
                           .Publish();
            }

            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Telefon numarasına ait kullanıcı adını verir
        /// </summary>
        /// <param name="phoneNumber">Telefon numarası</param>
        /// <returns>Eğer telefon numarası kayıtlı ise kullanıcı adı değilse null döner</returns>
        public async Task<string> GetUserNameByPhoneNumber(string phoneNumber)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/GetUserName?phoneNumber={phoneNumber}");

            var response = await _requestProvider.GetAsync<UserNameModel>(uri);
            if (response.IsSuccess)
            {
                return response.Data.UserName;
            }

            return null;
        }

        /// <summary>
        /// Şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Kullanıcı şifre bilgileri</param>
        public async Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ChangePassword");

            var response = await _requestProvider.PostAsync<string>(uri, changePasswordModel);
            if (!response.IsSuccess)
            {
                await ShowErrorMessage(response.Error.ErrorMessage);
            }

            return response.IsSuccess;
        }

        /// <summary>
        /// Kullanıcının dil ayarını günceller
        /// </summary>
        /// <param name="language">Seçilen dil</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateLanguageAsync(Languages language)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/UpdateLanguage?language={(byte)language}");

            var response = await _requestProvider.PostAsync<string>(uri);
            if (!response.IsSuccess && response.Error != null && !string.IsNullOrEmpty(response.Error.ErrorMessage))
            {
                await ShowErrorMessage(response.Error.ErrorMessage);
            }
            else
            {
                _analyticsService.SendEvent("Ayarlar", "Dil", language.ToLanguageCode());
            }

            return response.IsSuccess;
        }

        /// <summary>
        /// Şifremi unuttum kodu kontrol eder
        /// </summary>
        /// <param name="code">Kod</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> ChangePasswordAsync(int code)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ChangePassword/Codecheck?code={code}");

            var response = await _requestProvider.GetAsync<string>(uri);
            if (!response.IsSuccess)
            {
                await ShowErrorMessage(ContestParkResources.YouHaveEnteredAnInvalidCode);
            }

            return response.IsSuccess;
        }

        /// <summary>
        /// Kullanıcıya ait telefon numarası
        /// </summary>
        /// <returns>Telefon numarası</returns>
        public async Task<string> GetPhoneNumber()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/PhoneNumber");

            var response = await _requestProvider.GetAsync<PhoneNumberModel>(uri);
            if (!response.IsSuccess)
                return string.Empty;

            return response.Data.PhoneNumber;
        }

        /// <summary>
        /// Şifremi unuttum
        /// </summary>
        /// <param name="UserNameOrEmail">Kullanıcı adı veya eposta adresi</param>
        public async Task ForgetYourPasswordAsync(string UserNameOrEmail)
        {
            if (string.IsNullOrEmpty(UserNameOrEmail))
            {
                await ShowErrorMessage(ContestParkResources.WriteYourUserNameOrEmailAddress);
            }
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/ForgotYourPassword");

            var result = await _requestProvider.PostAsync<ValidationResultModel>(uri, new { UserNameOrEmail });

            await ShowErrorMessage(result.Data.ErrorMessage);
        }

        /// <summary>
        /// Kullanıcı id'sine göre profil sayfasında gösterilen dataları döndürür
        /// </summary>
        /// <param name="userName">Kullanıcı id</param>
        /// <returns>Profil sayfası görütülemek için gerekli bilgiler</returns>
        public async Task<ProfileInfoModel> GetProfileInfoByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/Profile/{userName}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ProfileInfoModel>(uri);
            }

            var result = await _requestProvider.GetAsync<ProfileInfoModel>(uri);
            if (result.Data != null && result.IsSuccess)
            {
                _cacheService.Add(uri, result.Data);
            }

            return result.Data;
        }

        /// <summary>
        /// Kullanıcı adı ve şifre ile token bilgisi alır
        /// </summary>
        /// <param name="loginModel">Kullanıcı login bilgileri</param>
        /// <returns>Token</returns>
        public async Task<UserToken> GetTokenAsync(LoginModel loginModel)
        {
            try
            {
                var from = new Dictionary<string, string>
                {
                    {"username", loginModel.UserName },
                    {"password", loginModel.Password },
                    {"client_id", GlobalSetting.ClientId },
                    {"grant_type", "password" },
                    {"scope", GlobalSetting.Scope },
                };

                var result = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, from);
                if (!result.IsSuccess)
                {
                    string translateMessage = TranslateExtension.resmgr.Value.GetString(result.Error.ErrorMessage);
                    await ShowErrorMessage(translateMessage);
                }

                return result.Data;
            }
            catch (HttpRequestExceptionEx ex)
            {
                await ShowHttpErrorMessage(ex);
            }
            catch (HttpRequestException ex)
            {
                await ShowErrorMessage(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Token refresh
        /// </summary>
        /// <returns></returns>
        public async Task RefreshTokenAsync()
        {
            try
            {
                string refreshToken = _settingsService.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken))
                    return;

                var from = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token" },
                    {"refresh_token", refreshToken },
                    {"client_id", GlobalSetting.ClientId },
                };

                var response = await _requestProvider.PostAsync<RefreshTokenModel>(GlobalSetting.Instance.TokenEndpoint, from);

                RefreshTokenModel refreshTokenModel = response.Data;
                if (refreshTokenModel != null)
                {
                    // Token bilgisi yenilendi
                    _settingsService.SetTokenInfo(new UserToken
                    {
                        AccessToken = refreshTokenModel.AccessToken,
                        RefreshToken = refreshTokenModel.RefreshToken,
                        TokenType = refreshTokenModel.TokenType
                    });
                }
            }
            catch (HttpRequestExceptionEx ex)
            {
                await ShowHttpErrorMessage(ex);
            }
            catch (HttpRequestException ex)
            {
                await ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Üye ol servisine istek atar başarılı ise true başarısız ise mesaj çıkarır ve false döndürür
        /// </summary>
        /// <param name="signUpModel">Üye olma bilgileri</param>
        /// <returns>Başarılı ise true başarısız ise false</returns>
        public async Task<bool> SignUpAsync(SignUpModel signUpModel)
        {
            if (signUpModel == null)
                return false;

            if (_settingsService.SignUpCount > 3)// Sürekli üye olup davetiye kodu ile para kasmasınlar diye bir cihazdan 3 kere üye olma hakkı verdik :)
            {
                await ShowErrorMessage(ContestParkResources.GlobalErrorMessage);

                return false;
            }

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, _apiUrlBase);

            // TODO: uygulama dili değişince nuradaki dil değişecek mi test edilmesi lazım
            CultureInfo cultureInfo = Xamarin.Forms.DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            signUpModel.LanguageCode = cultureInfo.IetfLanguageTag;

            signUpModel.DeviceIdentifier = Xamarin.Forms.DependencyService.Get<IDevice>().GetIdentifier();// IMEI numarası alındı

            var response = await _requestProvider.PostAsync<string>(uri, signUpModel);

            if (!string.IsNullOrEmpty(response.Error?.ErrorMessage))
                await ShowValidationMessages(new string[1] { response?.Error?.ErrorMessage });

            await ShowValidationMessages(response?.Error?.MemberNames);

            return response.IsSuccess;
        }

        /// <summary>
        /// Validation mesajlarını tek tek gösterir
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        private async Task ShowValidationMessages(string[] validationResult)
        {
            if (validationResult == null || validationResult.Length == 0)
                return;

            foreach (var item in validationResult)
            {
                await _dialogService.DisplayAlertAsync("", item, ContestParkResources.Okay);
            }
        }

        /// <summary>
        /// Çıkış yap
        /// </summary>
        public async Task Unauthorized()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.LogoutEndpoint);

            var response = await _requestProvider.PostAsync<string>(uri);

            _analyticsService.SendEvent("Login", "Logout", response.IsSuccess ? "Success" : "Fail");

            _settingsService.AuthAccessToken = string.Empty;
            _settingsService.SignalRConnectionId = string.Empty;

            _settingsService.RemoveCurrentUser();

            _cacheService.EmptyAll();

            await _signalRServiceBase.DisconnectAsync();
        }

        /// <summary>
        /// Kullanıcının kullanıcı adı ve ad soyad bilgisini günceller
        /// </summary>
        /// <param name="userInfo">Ad soyad ve kullanıcı adı modeli</param>
        public async Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/Update");

            var result = await _requestProvider.PostAsync<string>(uri, userInfo);

            await ShowValidationMessages(result.Error?.MemberNames);

            return result.IsSuccess;
        }

        private async Task ShowErrorMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
#if DEBUG
            await _dialogService.DisplayAlertAsync(
                          ContestParkResources.Error,
                     message,
                           ContestParkResources.Okay);
#else
            await _dialogService.DisplayAlertAsync(
                          ContestParkResources.Error,
                     ContestParkResources.GlobalErrorMessage,
                           ContestParkResources.Okay);
#endif
        }

        private async Task ShowHttpErrorMessage(Exception ex)
        {
            HttpRequestExceptionEx httpRequestExceptionEx = (HttpRequestExceptionEx)ex;

            if (httpRequestExceptionEx.HttpCode == System.Net.HttpStatusCode.BadRequest)
            {
                IdentityServerErorModel result = await Task.Run(() =>
              JsonConvert.DeserializeObject<IdentityServerErorModel>(ex.Message));

                string message = "";
#if DEBUG
                message = ex.Message;
#else
                message = ContestParkResources.GlobalErrorMessage;
#endif

                await ShowErrorMessage(message);
            }
        }

        #endregion Methods
    }
}
