using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.ViewModels.Base;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
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

        private double _timer = 10;

        public double Timer
        {
            get { return _timer; }
            set
            {
                _timer = value;
                RaisePropertyChanged(() => Timer);
            }
        }

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

        #region Methods

        protected override Task InitializeAsync()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () =>
               {
                   Timer -= 0.100;

                   if (Timer <= 0)
                       ClosePopupCommand.Execute(null);

                   return Timer > 0;
               });

            return base.InitializeAsync();
        }

        private async Task ExecuteAcceptDuelInviteCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await Task.Delay(3345);

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }
        public ICommand AcceptDuelInviteCommand { get { return new Command(async () => await ExecuteAcceptDuelInviteCommand()); } }

        #endregion Commands
    }
}
