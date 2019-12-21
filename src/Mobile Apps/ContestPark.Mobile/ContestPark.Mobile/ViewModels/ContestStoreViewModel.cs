using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ContestStoreViewModel : ViewModelBase<InAppBillingProductModel>
    {
        #region Private variables

        private readonly IBalanceService _balanceService;
        private readonly IAdMobService _adMobService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInAppBillingService _inAppBillingService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public ContestStoreViewModel(
            IPageDialogService pageDialogService,
            IInAppBillingService inAppBillingService,
            IBalanceService balanceService,
            IAdMobService adMobService,
            ISettingsService settingsService,
            IEventAggregator eventAggregator
            ) : base(dialogService: pageDialogService)
        {
            Title = ContestParkResources.ContestStore;
            _inAppBillingService = inAppBillingService;
            _balanceService = balanceService;
            _adMobService = adMobService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Properties

        public BalanceTypes BalanceType { get; set; } = BalanceTypes.Gold;
        public List<InAppBillingProductModel> Products { get; set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            _adMobService.OnRewardedVideoAdClosed += _adMobService_OnRewardedVideoAdClosed;

            Products = await _inAppBillingService.GetProductInfoAsync();
            if (Products != null)
                Items.AddRange(Products.Where(x => x.BalanceTypes == BalanceType).ToList());

            // await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Reklam izleme işlemi bitnce altın eklemesi için servere istek gönderir
        /// </summary>
        private async void _adMobService_OnRewardedVideoAdClosed(object sender, EventArgs e)
        {
            bool isSuccess = await _balanceService.RewardedVideoaAsync();
            if (!isSuccess)
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.AnErrorOccurredDuringThePrizeAwardingProcess,
                                        ContestParkResources.Okay);
            }
        }

        private Platforms GetCurrentPlatform()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android: return Platforms.Android;
                case Device.iOS: return Platforms.Ios;
            }

            return Platforms.Android;
        }

        /// <summary>
        /// Uygulama içi ürün satın al
        /// </summary>
        /// <param name="productId">Ürün id</param>
        private async Task ExecutePurchaseCommandAsync(string productId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var purchaseInfo = await _inAppBillingService.PurchaseAsync(productId);
            if (purchaseInfo == null)
            {
                IsBusy = false;

                return;
            }

            bool isSuccessGoldPurchase = await _balanceService.PurchaseAsync(new PurchaseModel
            {
                ProductId = purchaseInfo.ProductId,
                PackageName = purchaseInfo.ProductId,
                Token = purchaseInfo.PurchaseToken,
                Platform = GetCurrentPlatform()
            });
            if (isSuccessGoldPurchase)
            {
                await DisplayAlertAsync(
                    ContestParkResources.Success,
                    ContestParkResources.ThePurchaseIsSuccessfulYourGoldBasBeenUploadedToYourAccount,
                    ContestParkResources.Okay);

                // Left menü'deki  altın miktarını güncelledik
                _eventAggregator
                     .GetEvent<GoldUpdatedEvent>()
                     .Publish();
            }
            else
            {
                /*
                 Satın alma işlemi başarısız. Altınlarınız hesabınıza yüklenemedi. Lütfen support@contestpark.com adresine mail atın.
                 Purchase failed. Your gold could not be uploaded to your account. Please send an email to support@contestpark.com address.
                 */

                UserInfoModel userInfo = _settingsService.CurrentUser;

                Debug.WriteLine($@"Satın alma işlemi api tarafından başarısız oldu!
                                       UserId: {userInfo.UserId}
                                       UserName: {userInfo.UserName}
                                       Product Id: {purchaseInfo.ProductId}
                                       Id: {purchaseInfo.Id}
                                       Auto renewing: {purchaseInfo.AutoRenewing}
                                       Payload: {purchaseInfo.Payload}
                                       Purchase token: {purchaseInfo.PurchaseToken}
                                       Transaction date utc: {purchaseInfo.TransactionDateUtc}
                                       State: {purchaseInfo.State.ToString()}
                                       Consumption state: {purchaseInfo.ConsumptionState.ToString()}");

                await DisplayAlertAsync(
                    ContestParkResources.Error,
                    ContestParkResources.PurchaseFail,
                    ContestParkResources.Okay);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Reklam izle altın al
        /// </summary>
        private void ExecuteWatchAdsVideoCommand()
        {
            _adMobService.ShowOrLoadRewardedVideo();
        }

        private void ExecuteChangeBalanceTypeCommand(BalanceTypes balanceType)
        {
            if (IsBusy || BalanceType == balanceType)
                return;

            IsBusy = true;

            BalanceType = balanceType;

            Items.Clear();

            Items.AddRange(Products.Where(x => x.BalanceTypes == BalanceType).ToList());

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand purchaseCommand;
        private ICommand changeBalanceType;

        private ICommand watchAdsVideoCommand;

        public ICommand PurchaseCommand
        {
            get { return purchaseCommand ?? (purchaseCommand = new Command<string>(async (productId) => await ExecutePurchaseCommandAsync(productId))); }
        }

        public ICommand WatchAdsVideoCommand
        {
            get { return watchAdsVideoCommand ?? (watchAdsVideoCommand = new Command(() => ExecuteWatchAdsVideoCommand())); }
        }

        public ICommand ChangeBalanceType
        {
            get { return changeBalanceType ?? (changeBalanceType = new Command<string>((balanceType) => ExecuteChangeBalanceTypeCommand((BalanceTypes)Convert.ToByte(balanceType)))); }
        }

        public ICommand RemoveOnRewardedVideoAdClosed
        {
            get
            {
                return new Command(() => _adMobService.OnRewardedVideoAdClosed -= _adMobService_OnRewardedVideoAdClosed);
            }
        }

        #endregion Commands
    }
}
