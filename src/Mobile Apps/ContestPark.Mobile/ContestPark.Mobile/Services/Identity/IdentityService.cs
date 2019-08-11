using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.ErrorModel;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.RequestProvider;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Base;
using Newtonsoft.Json;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/Account";
        private readonly IPageDialogService _dialogService;
        private readonly INewRequestProvider _requestProvider;
        private readonly ISettingsService _settingsService;

        private readonly ISignalRServiceBase _signalRServiceBase;

        #endregion Private variables

        #region Constructor

        public IdentityService(
            INewRequestProvider requestProvider,
            IPageDialogService dialogService,
            ISettingsService settingsService,
            ISignalRServiceBase signalRServiceBase)
        {
            _requestProvider = requestProvider;
            _dialogService = dialogService;
            _settingsService = settingsService;
            _signalRServiceBase = signalRServiceBase;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kapak resmi değiştir
        /// </summary>
        /// <param name="picture">Resim stream</param>
        public async Task ChangeCoverPictureAsync(MediaModel media)
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/ChangeCoverPicture");

            var response = await _requestProvider.PostAsync<ValidationResultModel>(uri, media);

            await ShowValidationMessages(response.Data);

            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Kullanıcı şifre bilgileri</param>
        public async Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/ChangePassword");

            var response = await _requestProvider.PostAsync<ValidationResultModel>(uri, changePasswordModel);
            if (!response.IsSuccess)
            {
                await ShowErrorMessage(response.Data.ErrorMessage);
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
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/ChangePassword/Codecheck?code={code}");

            var response = await _requestProvider.GetAsync<string>(uri);
            if (!response.IsSuccess)
            {
                await ShowErrorMessage(ContestParkResources.YouHaveEnteredAnInvalidCode);
            }

            return response.IsSuccess;
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        /// <param name="picture">Resim stream</param>
        public async Task ChangeProfilePictureAsync(MediaModel media)
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/ChangeProfilePicture");

            var response = await _requestProvider.PostAsync<ValidationResultModel>(uri, media);

            await ShowValidationMessages(response.Data);

            UserDialogs.Instance.HideLoading();
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
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/ForgotYourPassword");

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

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/Profile/{userName}");

            var result = await _requestProvider.GetAsync<ProfileInfoModel>(uri);

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
                    {"client_id", GlobalSetting.Instance.ClientId },
                    {"grant_type", "password" },
                    {"scope", GlobalSetting.Instance.Scope },
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
                    {"client_id", GlobalSetting.Instance.ClientId },
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

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            // TODO: uygulama dili değişince nuradaki dil değişecek mi test edilmesi lazım
            CultureInfo cultureInfo = Xamarin.Forms.DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            signUpModel.LanguageCode = cultureInfo.IetfLanguageTag;

            var response = await _requestProvider.PostAsync<ValidationResultModel>(uri, signUpModel);

            await ShowValidationMessages(response.Data);

            return response.Data.MemberNames?.Count() == 0;
        }

        /// <summary>
        /// Validation mesajlarını tek tek gösterir
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        private async Task ShowValidationMessages(ValidationResultModel validationResult)
        {
            if (validationResult == null || validationResult.MemberNames == null)
                return;

            foreach (var item in validationResult.MemberNames)
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

            await _requestProvider.PostAsync<string>(uri);

            _settingsService.AuthAccessToken = string.Empty;
            _settingsService.SignalRConnectionId = string.Empty;

            _settingsService.RemoveCurrentUser();

            await _signalRServiceBase.DisconnectAsync();
        }

        /// <summary>
        /// Kullanıcının kullanıcı adı ve ad soyad bilgisini günceller
        /// </summary>
        /// <param name="userInfo">Ad soyad ve kullanıcı adı modeli</param>
        public async Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/Update");

            var result = await _requestProvider.PostAsync<string>(uri, userInfo);

            await ShowValidationMessages(result.Error);

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
