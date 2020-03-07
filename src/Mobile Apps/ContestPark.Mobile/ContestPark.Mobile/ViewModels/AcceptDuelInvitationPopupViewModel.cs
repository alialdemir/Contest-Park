using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.ViewModels.Base;
using Rg.Plugins.Popup.Contracts;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class AcceptDuelInvitationPopupViewModel : ViewModelBase
    {
        #region Constructor

        public AcceptDuelInvitationPopupViewModel(IPopupNavigation popupNavigation) : base(popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Properties

        private InviteModel _inviteModel;

        public InviteModel InviteModel
        {
            get
            {
                return _inviteModel;
            }
            set
            {
                _inviteModel = value;
                RaisePropertyChanged(() => InviteModel);
            }
        }

        #endregion Properties

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        #endregion Commands
    }
}
