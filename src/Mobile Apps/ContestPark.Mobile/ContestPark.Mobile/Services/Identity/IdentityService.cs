using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.RequestProvider;
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

        #endregion Private variables

        #region Constructor

        public IdentityService(
            IRequestProvider requestProvider,
            IPageDialogService dialogService)
        {
            _requestProvider = requestProvider;
            _dialogService = dialogService;
        }

        #endregion Constructor

        #region Methods

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
                    await ShowMessage(ex.Message);
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

                await ShowMessage(message);
            }
        }

        private async Task ShowMessage(string message)
        {
            await _dialogService.DisplayAlertAsync(
                             ContestParkResources.Error,
                        message,
                              ContestParkResources.Okay);
        }

        #endregion Methods
    }
}