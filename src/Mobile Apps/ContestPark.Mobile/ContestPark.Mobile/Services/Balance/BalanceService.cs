using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Services.RequestProvider;
using Prism.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public class BalanceService : IBalanceService
    {
        #region Private variables

        private const string _apiUrlBase = "api/v1/Balance";
        private readonly IRequestProvider _requestProvider;
        private readonly IPageDialogService _pageDialogService;

        #endregion Private variables

        #region Constructor

        public BalanceService(IRequestProvider requestProvider,
                              IPageDialogService pageDialogService)
        {
            _requestProvider = requestProvider;
            _pageDialogService = pageDialogService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Reklam izleyerek altın kazandı
        /// </summary>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> RewardedVideoaAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/RewardedVideo");

            var result = await _requestProvider.PostAsync<string>(uri);

            return result.IsSuccess;
        }

        /// <summary>
        /// Günlük altın kazanma hakkı
        /// </summary>
        public async Task<RewardModel> RewardAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/Reward");

            var result = await _requestProvider.PostAsync<RewardModel>(uri);
            if (!result.IsSuccess || result.Data == null)
                return new RewardModel();

            return result.Data;
        }

        /// <summary>
        /// Giriiş toplam altın miktarını verir
        /// </summary>
        /// <returns>Toplam altın miktarı</returns>
        public async Task<BalanceModel> GetBalanceAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, _apiUrlBase);

            var result = await _requestProvider.GetAsync<BalanceModel>(uri);
            if (!result.IsSuccess)
                return new BalanceModel();

            return result.Data;
        }

        /// <summary>
        /// Uygulama içi ürün satın aldığında
        /// </summary>
        /// <param name="productId">Ürün id</param>
        /// <returns>Altın miktarı eklendi ise true eklenemedi ise false döner</returns>
        public async Task<bool> PurchaseAsync(PurchaseModel purchase)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/Purchase/v2");

            var result = await _requestProvider.PostAsync<string>(uri, purchase);

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent($@"Satın alma işlemi service!", new Dictionary<string, string>
                {
                    { "HttpStatusCode", result.HttpStatusCode.ToString() },
                    { "IsSuccess", result.IsSuccess.ToString() },
                    { "ErrorMessage", result.Error.ErrorMessage },
                    { "MemberNames",result.Error != null && result.Error.MemberNames !=null ? result.Error.MemberNames.Length.ToString() :"0"},
                    { "Error",result.Error != null && result.Error.MemberNames !=null && result.Error.MemberNames.Length > 0 ? result.Error.MemberNames[0]:"hata yok" },
                });

            return result.IsSuccess;
        }

        /// <summary>
        /// Bakiye çekme isteği gönderir
        /// </summary>
        public async Task<bool> GetBalanceRequest(IbanNoModel ibanNo)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, _apiUrlBase);

            var result = await _requestProvider.PostAsync<string>(uri, ibanNo);
            if (!result.IsSuccess && result.Error != null)
            {
                await _pageDialogService?.DisplayAlertAsync("",
                                                            result.Error.ErrorMessage,
                                                            ContestParkResources.Okay);
            }

            return result.IsSuccess;
        }

        /// <summary>
        /// Hesaba bakiye kodu ile bakiye yükler
        /// </summary>
        /// <param name="balanceCodeModel">Bakiye kod bilgisi</param>
        /// <returns>Başarılı ise true değilse false döner</returns>
        public async Task<bool> BalanceCode(BalanceCodeModel balanceCodeModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{_apiUrlBase}/Code");

            var result = await _requestProvider.PostAsync<string>(uri, balanceCodeModel);

            if (!result.IsSuccess && result.Error != null)
            {
                await _pageDialogService?.DisplayAlertAsync("",
                                                            result.Error.ErrorMessage,
                                                            ContestParkResources.Okay);
            }

            return result.IsSuccess;
        }

        #endregion Methods
    }
}
