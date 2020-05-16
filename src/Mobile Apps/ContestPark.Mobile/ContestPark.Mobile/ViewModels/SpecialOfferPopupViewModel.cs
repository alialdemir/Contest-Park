using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.InAppBillingProduct;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SpecialOfferPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IAnalyticsService _analyticsService;

        private readonly IInAppBillingService _inAppBillingService;

        #endregion Private variables

        #region Constructor

        public SpecialOfferPopupViewModel(INavigationService navigationService,
                                          IAnalyticsService analyticsService,
                                          IInAppBillingService inAppBillingService) : base(navigationService)
        {
            _analyticsService = analyticsService;
            _inAppBillingService = inAppBillingService;
        }

        #endregion Constructor

        #region Properties

        private InAppBillingProductModel _product;

        public InAppBillingProductModel Product
        {
            get { return _product; }
            set
            {
                _product = value;

                RaisePropertyChanged(() => Product);
            }
        }

        public DateTime Now { get; } = DateTime.Now.AddMinutes(10);

        private double _timer = 600;// 10 dakkika olduğu için 600 saniye değerini verdik

        public double Timer
        {
            get { return _timer; }
            set
            {
                _timer = value;
                RaisePropertyChanged(() => Timer);
            }
        }

        private string _timeText;

        public string TimeText
        {
            get { return _timeText; }
            set
            {
                _timeText = value;
                RaisePropertyChanged(() => TimeText);
            }
        }

        private bool IsExit { get; set; }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            InitSpecialOfferCommand.Execute(null);

            TimerCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Özel indirimli ürünü alır
        /// </summary>
        private async Task ExecuteInitSpecialOfferCommandAsync()
        {
            var product = await _inAppBillingService.GetProductById(_inAppBillingService.SpecialProductId);
            if (product != null)
            {
                product.Title = ContestParkResources.SpecialOffer;
                product.Description = ContestParkResources.GetYourSpecialDiscountPackageNow;

                Product = product;
            }
            else
                GotoBackCommand.Execute(true);
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            IsExit = true;

            return base.GoBackAsync(parameters, useModalNavigation);
        }

        /// <summary>
        /// Kampanyanın kalan süresini hesaplar
        /// </summary>
        private void ExecuteTimerCommand()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
            {
                if (IsExit)
                    return false;

                Timer -= 1;

                TimeSpan diff = Now - DateTime.Now;

                TimeText = string.Format(ContestParkResources.RemainingTimeSec, diff.Minutes, diff.Seconds);//$"Kalan Süre: {diff.Minutes}m {diff.Seconds}s";

                if (Timer <= 0)
                    GotoBackCommand.Execute(true);

                return Timer > 0;
            });
        }

        /// <summary>
        /// Satın alma işlemi gerçekleştirir
        /// </summary>
        /// <returns></returns>
        private void ExecuteBuyCommand()
        {
            _inAppBillingService.PurchaseProcessAsync(_inAppBillingService.SpecialProductId);

            _analyticsService.SendEvent("ContestStore", "Özel ürün", "Düello sonuç ekranı");

            GotoBackCommand.Execute(true);
        }

        #endregion Methods

        #region Commands

        private ICommand InitSpecialOfferCommand => new CommandAsync(ExecuteInitSpecialOfferCommandAsync);
        private ICommand TimerCommand => new Command(ExecuteTimerCommand);

        public ICommand BuyCommand => new Command(ExecuteBuyCommand);

        #endregion Commands
    }
}
