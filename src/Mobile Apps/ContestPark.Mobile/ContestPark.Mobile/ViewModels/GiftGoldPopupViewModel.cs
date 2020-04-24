using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Services.BackgroundAggregator;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class GiftGoldPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IBackgroundAggregatorService _backgroundAggregatorService;

        #endregion Private variables

        #region Constructor

        public GiftGoldPopupViewModel(IPopupNavigation popupNavigation,
                                      INavigationService navigationService,
                                      IBackgroundAggregatorService backgroundAggregatorService) : base(navigationService, popupNavigation: popupNavigation)
        {
            _backgroundAggregatorService = backgroundAggregatorService;
        }

        #endregion Constructor

        #region Properties

        private string _gift = "gift1.gif";

        private RewardModel _giftGold;

        public RewardModel GiftGold
        {
            get { return _giftGold; }
            set
            {
                _giftGold = value;
                RaisePropertyChanged(() => GiftGold);
            }
        }

        public string Gift
        {
            get { return _gift; }
            set
            {
                _gift = value;
                RaisePropertyChanged(() => Gift);
            }
        }

        #endregion Properties

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("RewardModel"))
                GiftGold = parameters.GetValue<RewardModel>("RewardModel");

            return base.InitializeAsync(parameters);
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            // Bir sonraki ödül için push notification göndermesi için background job başlattık
            _backgroundAggregatorService.StartRewardJob(GiftGold.NextRewardTime);

            return base.GoBackAsync(parameters, useModalNavigation: true);
        }

        #endregion Methods

        #region Command

        public ICommand ClickGiftCommand
        {
            get
            {
                return new Command(() =>
                {
                    Gift = "gift2.gif";
                });
            }
        }

        #endregion Command
    }
}
