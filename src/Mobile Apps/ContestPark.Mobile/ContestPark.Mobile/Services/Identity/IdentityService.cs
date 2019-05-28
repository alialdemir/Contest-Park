using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.ErrorModel;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.Login;
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/account";
        private readonly IPageDialogService _dialogService;
        private readonly IRequestProvider _requestProvider;
        private readonly ISettingsService _settingsService;

        private readonly ISignalRServiceBase _signalRServiceBase;

        #endregion Private variables

        #region Constructor

        public IdentityService(
            IRequestProvider requestProvider,
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
        public Task ChangeCoverPictureAsync(Stream picture)// TODO: URL düzeltilecek
        {
            //string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            //string message = await _requestProvider.PostAsync<string>(uri,picture);
            //await ShowErrorMessage(message);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Şifre değiştir
        /// </summary>
        /// <param name="changePasswordModel">Kullanıcı şifre bilgileri</param>
        public Task<bool> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            //string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            //string message = await _requestProvider.PostAsync<string>(uri, changePasswordModel);

            //await ShowErrorMessage(message);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Profil resmi değiştir
        /// </summary>
        /// <param name="picture">Resim stream</param>
        public async Task ChangeProfilePictureAsync(Stream picture)// TODO: URL düzeltilecek
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            string message = await _requestProvider.PostAsync<string>(uri, picture);

            await ShowErrorMessage(message);
        }

        /// <summary>
        /// Şifremi unuttum
        /// </summary>
        /// <param name="userNameOrEmailAddress">Kullanıcı adı veya eposta adresi</param>
        public async Task ForgetYourPasswordAsync(string userNameOrEmailAddress)
        {
            if (string.IsNullOrEmpty(userNameOrEmailAddress))
            {
                await ShowErrorMessage(ContestParkResources.WriteYourUserNameOrEmailAddress);
            }
            //string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            //string message = await _requestProvider.PostAsync<string>(uri, new { userNameOrEmailAddress });

            //await ShowErrorMessage(message);
        }

        /// <summary>
        /// Kullanıcı id'sine göre profil sayfasında gösterilen dataları döndürür
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Profil sayfası görütülemek için gerekli bilgiler</returns>
        public async Task<ProfileInfoModel> GetProfileInfoByUserName(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/{userId}");

            return await _requestProvider.GetAsync<ProfileInfoModel>(uri);
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

                return await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, from);
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

                RefreshTokenModel refreshTokenModel = await _requestProvider.PostAsync<RefreshTokenModel>(GlobalSetting.Instance.TokenEndpoint, from);
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
            signUpModel.LanguageCode = cultureInfo.TwoLetterISOLanguageName;

            ValidationResultModel validationResult = await _requestProvider.PostAsync<ValidationResultModel>(uri, signUpModel);

            if (validationResult != null)
            {
                foreach (var item in validationResult.MemberNames)
                {
                    await _dialogService.DisplayAlertAsync("", item, ContestParkResources.Okay);
                }
            }

            return validationResult.MemberNames.Count() == 0;
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
        public Task<bool> UpdateUserInfoAsync(UpdateUserInfoModel userInfo)
        {
            //string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            //string message = await _requestProvider.PostAsync<string>(uri, userInfo);

            //await ShowErrorMessage(message);
            return Task.FromResult(true);
        }

        private async Task ShowErrorMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
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

                if (result.ErrorDescription.Contains("invalid_username_or_password"))
                {
                    message = TranslateExtension.resmgr.Value.GetString(result.ErrorDescription);
                }

                await ShowErrorMessage(message);
            }
        }

        #endregion Methods
    }
}