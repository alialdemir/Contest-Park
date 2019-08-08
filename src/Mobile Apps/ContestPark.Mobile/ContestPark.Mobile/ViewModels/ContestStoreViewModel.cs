using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ContestStoreViewModel : ViewModelBase<InAppBillingProductModel>
    {
        #region Private variables

        private readonly IBalanceService _balanceService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInAppBillingService _inAppBillingService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public ContestStoreViewModel(
            IPageDialogService pageDialogService,
            IInAppBillingService inAppBillingService,
            IBalanceService cpService,
            ISettingsService settingsService,
            IEventAggregator eventAggregator
            ) : base(dialogService: pageDialogService)
        {
            Title = ContestParkResources.ContestStore;
            _inAppBillingService = inAppBillingService;
            _balanceService = cpService;
            _settingsService = settingsService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var products = await _inAppBillingService.GetProductInfoAsync();
            if (products != null)
                Items.AddRange(products);

            IsBusy = false;

            await base.InitializeAsync();
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
            var purchaseInfo = await _inAppBillingService.PurchaseAsync(productId);
            if (purchaseInfo == null)
                return;

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
        }

        /// <summary>
        /// Reklam izle altın al
        /// </summary>
        private async Task ExecuteWatchAdsVideoCommandAsync()
        {
            await DisplayAlertAsync("",
                "Coming soon!",
                ContestParkResources.Okay);
        }

        #endregion Methods

        #region Commands

        private ICommand purchaseCommand;

        private ICommand watchAdsVideoCommand;

        public ICommand PurchaseCommand
        {
            get { return purchaseCommand ?? (purchaseCommand = new Command<string>(async (productId) => await ExecutePurchaseCommandAsync(productId))); }
        }

        public ICommand WatchAdsVideoCommand
        {
            get { return watchAdsVideoCommand ?? (watchAdsVideoCommand = new Command(async () => await ExecuteWatchAdsVideoCommandAsync())); }
        }

        #endregion Commands
    }
}
