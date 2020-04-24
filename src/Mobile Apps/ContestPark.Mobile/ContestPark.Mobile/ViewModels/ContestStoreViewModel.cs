using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IAnalyticsService _analyticsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IInAppBillingService _inAppBillingService;

        #endregion Private variables

        #region Constructors

        public ContestStoreViewModel(
            IPageDialogService pageDialogService,
            IInAppBillingService inAppBillingService,
            IBalanceService balanceService,
            IAdMobService adMobService,
            IAnalyticsService analyticsService,
            IEventAggregator eventAggregator
            ) : base(dialogService: pageDialogService)
        {
            Title = ContestParkResources.ContestStore;
            _inAppBillingService = inAppBillingService;
            _balanceService = balanceService;
            _adMobService = adMobService;
            _analyticsService = analyticsService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Properties

        private string PurchaseTokenFilePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cp.txt"); }
        }

        private BalanceTypes _balanceType = BalanceTypes.Money;

        public BalanceTypes BalanceType
        {
            get { return _balanceType; }
            set
            {
                _balanceType = value;

                RaisePropertyChanged(() => BalanceType);
            }
        }

        public List<InAppBillingProductModel> Products { get; set; }

        #endregion Properties

        #region Methods

        protected override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (IsBusy)
                return Task.CompletedTask;

            IsBusy = true;

            _adMobService.OnRewardedVideoAdClosed += OnRewardedVideoAdClosed;

            GetProductCommand.Execute(null);

            IsBusy = false;

            return base.InitializeAsync(parameters);
        }

        /// <summary>
        /// Reklam izleme işlemi bitnce altın eklemesi için servere istek gönderir
        /// </summary>
        private async void OnRewardedVideoAdClosed(object sender, EventArgs e)
        {
            _analyticsService.SendEvent("ContestStore", "Ücretsiz Altın", "Ücretsiz Altın");

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

            string productName = Items.FirstOrDefault(x => x.ProductId == productId).ProductName;

            _analyticsService.SendEvent("Enhanced ECommerce", "Impression", productName);

            InAppBillingPurchaseModel purchaseInfo = await _inAppBillingService.PurchaseAsync(productId);
            if (purchaseInfo == null)
            {
                IsBusy = false;

                return;
            }

            #region Ios purchase token çok uzun olduğu için text dosyasına yazıp gönderiyoruz

            File.WriteAllText(PurchaseTokenFilePath, purchaseInfo.PurchaseToken);
            byte[] purchaseTokenByte = File.ReadAllBytes(PurchaseTokenFilePath);
            Stream purchaseTokenStream = new MemoryStream(purchaseTokenByte);

            #endregion Ios purchase token çok uzun olduğu için text dosyasına yazıp gönderiyoruz

            bool isSuccessGoldPurchase = await _balanceService.PurchaseAsync(new PurchaseModel
            {
                ProductId = purchaseInfo.ProductId,
                PackageName = purchaseInfo.ProductId,
                State = purchaseInfo.State,
                TransactionId = purchaseInfo.Id,
                Platform = GetCurrentPlatform(),
                File = purchaseTokenStream,
                FileName = "cp.conteststore"
            });
            if (isSuccessGoldPurchase)
            {
                PurchaseSuccess(purchaseInfo, productName);
            }
            else
            {
                await DisplayAlertAsync(
                                 ContestParkResources.Error,
                                 ContestParkResources.PurchaseFail,
                                 ContestParkResources.Okay);

                if (File.Exists(PurchaseTokenFilePath))
                    File.Delete(PurchaseTokenFilePath);

                SendProductEvent(purchaseInfo, "Remove From Cart", productName);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Satın alma işlemi başarılıysa yapılanlar
        /// </summary>
        /// <param name="purchaseInfo">Satılan paket bilgisi</param>
        /// <param name="productName">Paket adı</param>
        private void PurchaseSuccess(InAppBillingPurchaseModel purchaseInfo, string productName)
        {
            DisplayAlertAsync(
                            ContestParkResources.Success,
                            ContestParkResources.ThePurchaseIsSuccessfulYourGoldBasBeenUploadedToYourAccount,
                            ContestParkResources.Okay);

            // Left menü'deki  altın miktarını güncelledik
            _eventAggregator
                 .GetEvent<GoldUpdatedEvent>()
                 .Publish();

            SendProductEvent(purchaseInfo, "Purchase", productName);

            if (File.Exists(PurchaseTokenFilePath))
                File.Delete(PurchaseTokenFilePath);

            _inAppBillingService.ConsumePurchaseAsync(purchaseInfo.ProductId, purchaseInfo.PurchaseToken);
        }

        /// <summary>
        /// Product ga eventi gönderir
        /// </summary>
        /// <param name="purchaseInfo"></param>
        /// <param name="eventAction"></param>
        /// <param name="eventLabel"></param>
        private void SendProductEvent(InAppBillingPurchaseModel purchaseInfo, string eventAction, string eventLabel)
        {
            _analyticsService.SendEvent("Enhanced ECommerce", new Dictionary<string, string>
                {
                    { "ea", eventAction },
                    { "el", eventLabel },
                    { "ProductId", purchaseInfo.ProductId },
                    { "Id", purchaseInfo.Id },
                    { "AutoRenewing", purchaseInfo.AutoRenewing.ToString() },
                    { "Payload", purchaseInfo.Payload },
                    //{ "PurchaseToken", purchaseInfo.PurchaseToken },
                    { "TransactionDateUtc", purchaseInfo.TransactionDateUtc.ToLongDateString()},
                    { "State", purchaseInfo.State.ToString() },
                    { "ConsumptionState", purchaseInfo.ConsumptionState.ToString() },
                });
        }

        /// <summary>
        /// Reklam izle altın al
        /// </summary>
        private void ExecuteWatchAdsVideoCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            _adMobService.ShowOrLoadRewardedVideo();

            IsBusy = false;
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

        private ICommand _purchaseCommand;
        private ICommand _changeBalanceType;

        private ICommand _watchAdsVideoCommand;

        public ICommand PurchaseCommand
        {
            get { return _purchaseCommand ?? (_purchaseCommand = new CommandAsync<string>(ExecutePurchaseCommandAsync)); }
        }

        public ICommand WatchAdsVideoCommand
        {
            get { return _watchAdsVideoCommand ?? (_watchAdsVideoCommand = new Command(ExecuteWatchAdsVideoCommand)); }
        }

        public ICommand ChangeBalanceType
        {
            get { return _changeBalanceType ?? (_changeBalanceType = new Command<string>((balanceType) => ExecuteChangeBalanceTypeCommand((BalanceTypes)Convert.ToByte(balanceType)))); }
        }

        private byte _getProductCount = 0;

        private ICommand GetProductCommand
        {
            get => new Command(async () =>
            {
                Products = await _inAppBillingService.GetProductInfoAsync();
                if (Products != null && Products.Any())
                    Items.AddRange(Products.Where(x => x.BalanceTypes == BalanceType).ToList());
                else if (_getProductCount <= 10)
                {
                    _getProductCount += 1;

                    GetProductCommand.Execute(null);
                }
            });
        }

        public ICommand RemoveOnRewardedVideoAdClosed
        {
            get
            {
                return new Command(() => _adMobService.OnRewardedVideoAdClosed -= OnRewardedVideoAdClosed);
            }
        }

        #endregion Commands
    }
}
