using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
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
using System.Linq;
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
        }

        #endregion Constructors

        #region Properties

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

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            _adMobService.OnRewardedVideoAdClosed += OnRewardedVideoAdClosed;

            GetProductCommand.Execute(null);

            IsBusy = false;

            base.Initialize(parameters);
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

        /// <summary>
        /// Bakiye tipine göre ürün listesini değiştir
        /// </summary>
        /// <param name="balanceType">Bakiye tipi</param>
        private void ExecuteChangeBalanceTypeCommand(BalanceTypes balanceType)
        {
            if (IsBusy || BalanceType == balanceType || Items == null || !Items.Any())
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
            get { return _purchaseCommand ?? (_purchaseCommand = new CommandAsync<string>(_inAppBillingService.PurchaseProcessAsync)); }
        }

        public ICommand WatchAdsVideoCommand
        {
            get { return _watchAdsVideoCommand ?? (_watchAdsVideoCommand = new Command(ExecuteWatchAdsVideoCommand)); }
        }

        public ICommand ChangeBalanceType
        {
            get { return _changeBalanceType ?? (_changeBalanceType = new Command<byte>((balanceType) => ExecuteChangeBalanceTypeCommand((BalanceTypes)balanceType))); }
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
