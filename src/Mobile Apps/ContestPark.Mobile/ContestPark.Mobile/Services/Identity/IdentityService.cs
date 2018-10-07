using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.RequestProvider;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Base;
using Newtonsoft.Json;
using Prism.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Private variables

        private readonly IRequestProvider _requestProvider;

        private readonly IPageDialogService _dialogService;

        private readonly ISettingsService _settingsService;

        private readonly ISignalRServiceBase _signalRServiceBase;

        private const string ApiUrlBase = "api/v1/account";

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
        /// Üye ol servisine istek atar başarılı ise true başarısız ise mesaj çıkarır ve false döndürür
        /// </summary>
        /// <param name="signUpModel">Üye olma bilgileri</param>
        /// <returns>Başarılı ise true başarısız ise false</returns>
        public async Task<bool> SignUpAsync(SignUpModel signUpModel)
        {
            await Task.Delay(1000);
            return false;
        }

        /// <summary>
        /// Şifremi unuttum
        /// </summary>
        /// <param name="userNameOrEmailAddress">Kullanıcı adı veya eposta adresi</param>
        public Task ForgetYourPasswordAsync(string userNameOrEmailAddress)
        {
            //string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            //string message = await _requestProvider.PostAsync<string>(uri, new { userNameOrEmailAddress });

            //await ShowErrorMessage(message);
            return Task.CompletedTask;
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
                return await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint,
                    GlobalSetting.Instance.ClientId,
                    GlobalSetting.Instance.Scope, loginModel);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(HttpRequestExceptionEx))
                {
                    await ShowHttpErrorMessage(ex);
                }
                else if (ex.GetType() == typeof(HttpRequestException))
                {
#if DEBUG
                    await ShowErrorMessage(ex.Message);
#else
       await ShowMessage(ContestParkResources.GlobalErrorMessage);
#endif
                }
            }

            return null;
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

        private async Task ShowErrorMessage(string message)
        {
            await _dialogService.DisplayAlertAsync(
                             ContestParkResources.Error,
                        message,
                              ContestParkResources.Okay);
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

        #endregion Methods
    }
}