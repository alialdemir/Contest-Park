using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        #region Private variables

        private string userId;

        #endregion Private variables

        #region Constructor

        public ProfileViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = ContestParkResources.Profile;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Takip edenler listesine yönlendirir
        /// </summary>
        private void ExecuteGotoFollowersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PushNavigationPageAsync(nameof(FollowersView), new NavigationParameters
                {
                    {"UserId", userId}
                });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand _gotoFollowersCommand { get; set; }

        public ICommand GotoFollowersCommand
        {
            get { return _gotoFollowersCommand ?? (_gotoFollowersCommand = new Command(() => ExecuteGotoFollowersCommand())); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("UserId")) userId = parameters.GetValue<string>("UserId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}