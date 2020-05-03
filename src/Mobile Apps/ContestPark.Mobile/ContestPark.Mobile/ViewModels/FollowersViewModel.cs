using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class FollowersViewModel : ViewModelBase<FollowModel>
    {
        #region Private variables

        private readonly IFollowService _followService;
        private readonly IAnalyticsService _analyticsService;
        private string userId;

        #endregion Private variables

        #region Constructor

        public FollowersViewModel(
                INavigationService navigationService,
                IPageDialogService dialogService,
                IFollowService followService,
                IAnalyticsService analyticsService
            ) : base(navigationService, dialogService)
        {
            _followService = followService;
            _analyticsService = analyticsService;
            Title = ContestParkResources.Followers;
        }

        #endregion Constructor

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("UserId")) userId = parameters.GetValue<string>("UserId");

            FollowersCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Kullanıcı takip et takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        private async Task ExecuteFollowCommand(string userId)
        {
            if (IsBusy || string.IsNullOrEmpty(userId))
                return;

            FollowModel followModel = Items.Where(x => x.UserId == userId).FirstOrDefault();
            if (followModel == null)
                return;

            IsBusy = true;

            _analyticsService.SendEvent("Takipleşme", followModel.IsFollowing
                ? "Takipçiler - Takip Et"
                : "Takipçiler - Takipten Çıkart"
                , $"{followModel.UserId}");

            Items.Where(x => x.UserId == userId).First().IsFollowing = !followModel.IsFollowing;

            bool isSuccesss = await (followModel.IsFollowing ?
                  _followService.FollowUpAsync(userId) :
                  _followService.UnFollowAsync(userId));

            if (!isSuccesss)
            {
                Items.Where(x => x.UserId == userId).First().IsFollowing = !followModel.IsFollowing;

                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Profil sayfasına yönlendirir
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            NavigateToAsync<ProfileView>(new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Get followers list
        /// </summary>
        private async Task ExecuteFollowCommandAsync()
        {
            ServiceModel = await _followService.Followers(userId, ServiceModel);
        }

        #endregion Methods

        #region Commands

        private ICommand FollowersCommand => new CommandAsync(ExecuteFollowCommandAsync);

        private ICommand _followCommand;

        private ICommand _gotoProfilePageCommand;

        public ICommand FollowCommand
        {
            get { return _followCommand ?? (_followCommand = new CommandAsync<string>(ExecuteFollowCommand)); }
        }

        public ICommand GotoProfilePageCommand
        {
            get { return _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(ExecuteGotoProfilePageCommand)); }
        }

        #endregion Commands
    }
}
