using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpReferenceCodeViewModel : ViewModelBase
    {
        #region Constructor

        public SignUpReferenceCodeViewModel(INavigationService navigationService) : base(navigationService: navigationService)
        {
        }

        #endregion Constructor

        #region Properties

        private string _referenceCode;

        public string ReferenceCode
        {
            get { return _referenceCode; }
            set
            {
                _referenceCode = value;
                RaisePropertyChanged(() => ReferenceCode);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Referans kodu gir poupp kapatır
        /// </summary>
        private void ExecuteReferenceCodeCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            GoBackAsync(new NavigationParameters
            {
                { "ReferenceCode", ReferenceCode }
            }, true);

            IsBusy = false;
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            return base.GoBackAsync(parameters, true);
        }

        #endregion Methods

        #region Commands

        public ICommand ReferenceCodeCommand => new Command(ExecuteReferenceCodeCommand);

        #endregion Commands
    }
}
