using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class NotificationViewModel : ViewModelBase<NotificationModel>
    {
        #region Private variables

        private readonly INotificationService _notificationService;
        private readonly IFollowService _followService;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Notifications

        public NotificationViewModel(INotificationService notificationService,
                                     INavigationService navigationService,
                                     IPageDialogService dialogService,
                                     IFollowService followService,
                                     IAnalyticsService analyticsService) : base(navigationService, dialogService)
        {
            Title = ContestParkResources.Notifications;

            _notificationService = notificationService;
            _followService = followService;
            _analyticsService = analyticsService;
        }

        #endregion Notifications

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _notificationService.NotificationsAsync(ServiceModel);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcı takip et takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        private async Task ExecuteFollowCommand(string userId)
        {
            if (IsBusy || string.IsNullOrEmpty(userId))
                return;

            var notificationModel = Items.Where(x => x.UserId == userId).FirstOrDefault();
            if (notificationModel == null)
                return;

            IsBusy = true;

            _analyticsService.SendEvent("Takipleşme", notificationModel.IsFollowing
                ? "Bildirimler - Takip Et"
                : "Bildirimler - Takipten Çıkart"
                , $"{notificationModel.UserId}");

            Items.Where(x => x.UserId == userId).First().IsFollowing = !notificationModel.IsFollowing;

            bool isSuccesss = await (notificationModel.IsFollowing ?
                  _followService.FollowUpAsync(userId) :
                  _followService.UnFollowAsync(userId));

            if (!isSuccesss)
            {
                Items.Where(x => x.UserId == userId).First().IsFollowing = !notificationModel.IsFollowing;

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

            PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Post deyay sayfasına git
        /// </summary>
        private void ExecuteGotoPostDetailCommand(int postId)
        {
            if (IsBusy || postId <= 0)
                return;

            IsBusy = true;

            PushNavigationPageAsync(nameof(PostDetailView), new NavigationParameters
            {
                { "PostId", postId }
            });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand _followCommand;
        public ICommand _gotoProfilePageCommand;

        public ICommand FollowCommand
        {
            get { return _followCommand ?? (_followCommand = new Command<string>(async (userId) => await ExecuteFollowCommand(userId))); }
        }

        public ICommand GotoProfilePageCommand
        {
            get { return _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>((userName) => ExecuteGotoProfilePageCommand(userName))); }
        }

        public ICommand _gotoPostDetailCommand;

        public ICommand GotoPostDetailCommand
        {
            get { return _gotoPostDetailCommand ?? (_gotoPostDetailCommand = new Command<int>(async (postId) => await ExecuteGotoPostDetailCommand(postId))); }
        }

        #endregion Commands
    }
}
