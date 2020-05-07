using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class GiftGoldPopupViewModel : ViewModelBase
    {
        #region Constructor

        public GiftGoldPopupViewModel(IPopupNavigation popupNavigation,
                                      INavigationService navigationService) : base(navigationService, popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Properties

        private ImageSource _gift = "gift1.gif".ToResourceImage();

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

        public ImageSource Gift
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

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("RewardModel"))
                GiftGold = parameters.GetValue<RewardModel>("RewardModel");

            base.Initialize(parameters);
        }

        #endregion Methods

        #region Command

        public ICommand ClickGiftCommand
        {
            get
            {
                return new Command(() =>
                {
                    Gift = "gift2.gif".ToResourceImage();
                });
            }
        }

        #endregion Command
    }
}
