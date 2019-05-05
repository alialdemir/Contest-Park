using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Services.Follow;
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
    public class FollowingViewModel : ViewModelBase<FollowModel>
    {
        #region Private variables

        private readonly IFollowService _followService;
        private string userId;

        #endregion Private variables

        #region Constructor

        public FollowingViewModel(
                INavigationService navigationService,
                IPageDialogService dialogService,
                IFollowService followService
            ) : base(navigationService, dialogService)
        {
            _followService = followService;
            Title = ContestParkResources.Following;
        }

        #endregion Constructor

        #region Methods

        protected override async Task InitializeAsync()
        {
            ServiceModel = await _followService.Followers(userId, ServiceModel);

            await base.InitializeAsync();
        }

        /// <summary>
        /// Kullanıcı takip et takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        private async Task ExecuteFollowCommand(string userId)
        {
            if (IsBusy || string.IsNullOrEmpty(userId))
                return;

            IsBusy = true;

            FollowModel followModel = Items.Where(x => x.UserId == userId).First();
            if (followModel == null)
                return;

            Items.Where(x => x.UserId == userId).First().IsFollowing = !followModel.IsFollowing;

            bool isSuccesss = await (followModel.IsFollowing == true ?
                  _followService.UnFollowAsync(userId) :
                  _followService.FollowUpAsync(userId));

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

            PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand _followCommand { get; set; }
        public ICommand _gotoProfilePageCommand { get; set; }

        public ICommand FollowCommand
        {
            get { return _followCommand ?? (_followCommand = new Command<string>(async (userId) => await ExecuteFollowCommand(userId))); }
        }

        public ICommand GotoProfilePageCommand
        {
            get { return _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>((userName) => ExecuteGotoProfilePageCommand(userName))); }
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