using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpReferenceCodeViewModel : ViewModelBase
    {
        #region Constructor

        public SignUpReferenceCodeViewModel(IPopupNavigation popupNavigation) : base(popupNavigation: popupNavigation)
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
                { "ReferenceCode", "ReferenceCode" }
            }, true);

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command<PopupPage>(async (popupPage) => await RemovePopupPageAsync(popupPage)); } }
        public ICommand ReferenceCodeCommand => new Command(() => ExecuteReferenceCodeCommand());

        #endregion Commands
    }
}
