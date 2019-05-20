using ContestPark.Mobile.ViewModels.Base;
using Rg.Plugins.Popup.Contracts;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PhotoModalViewModel : ViewModelBase
    {
        #region Constructor

        public PhotoModalViewModel(
            IPopupNavigation popupNavigation) : base(popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        #endregion Commands
    }
}