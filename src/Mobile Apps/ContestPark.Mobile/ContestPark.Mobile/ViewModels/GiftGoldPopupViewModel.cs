using ContestPark.Mobile.ViewModels.Base;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class GiftGoldPopupViewModel : ViewModelBase
    {
        #region Constructor

        public GiftGoldPopupViewModel(IPopupNavigation popupNavigation) : base(popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Properties

        private string _gift = "gift1.gif";

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

        protected override Task InitializeAsync()
        {
            return base.InitializeAsync();
        }

        #endregion Methods

        #region Command

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

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
