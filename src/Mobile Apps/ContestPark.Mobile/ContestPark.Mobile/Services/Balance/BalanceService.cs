﻿using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Services.RequestProvider;
using Prism.Services;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Cp
{
    public class BalanceService : IBalanceService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/Balance";
        private readonly INewRequestProvider _requestProvider;
        private readonly IPageDialogService _pageDialogService;

        #endregion Private variables

        #region Constructor

        public BalanceService(INewRequestProvider requestProvider,
                              IPageDialogService pageDialogService)
        {
            _requestProvider = requestProvider;
            _pageDialogService = pageDialogService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Giriiş toplam altın miktarını verir
        /// </summary>
        /// <returns>Toplam altın miktarı</returns>
        public async Task<BalanceModel> GetTotalCpByUserIdAsync()
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            var result = await _requestProvider.GetAsync<BalanceModel>(uri);

            return result.Data;
        }

        /// <summary>
        /// Uygulama içi ürün satın aldığında
        /// </summary>
        /// <param name="productId">Ürün id</param>
        /// <returns>Altın miktarı eklendi ise true eklenemedi ise false döner</returns>
        public async Task<bool> PurchaseAsync(PurchaseModel purchase)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, ApiUrlBase);

            var result = await _requestProvider.PostAsync<string>(uri, purchase);

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