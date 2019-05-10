using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ProfileViewModel : ViewModelBase<PostModel>
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly IPostService _postService;
        private string userName;

        #endregion Private variables

        #region Constructor

        public ProfileViewModel(
            INavigationService navigationService,
            IPageDialogService dialogService,
            IIdentityService identityService,
            IPostService postService

            ) : base(navigationService, dialogService)
        {
            _identityService = identityService;
            _postService = postService;
            NavigationService = navigationService;
            Title = ContestParkResources.Profile;
        }

        #endregion Constructor

        #region Properties

        private bool isVisibleBackArrow = false;
        private ProfileInfoModel profileInfo;

        public bool IsVisibleBackArrow
        {
            get { return isVisibleBackArrow; }
            set
            {
                isVisibleBackArrow = value;
                RaisePropertyChanged(() => IsVisibleBackArrow);
            }
        }

        public INavigationService NavigationService { get; }

        /// <summary>
        /// Kullanıcı profil bilgileri
        /// </summary>
        public ProfileInfoModel ProfileInfo
        {
            get { return profileInfo; }
            set
            {
                profileInfo = value;
                RaisePropertyChanged(() => ProfileInfo);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            var profileInfo = await _identityService.GetProfileInfoByUserName(userName);

            if (profileInfo != null)
            {
                ProfileInfo = profileInfo;
                ServiceModel = await _postService.GetPostsByUserIdAsync(ProfileInfo.UserId, ServiceModel);
            }
            else
            {
                await DisplayAlertAsync("",
                    ContestParkResources.UserNotFound,
                    ContestParkResources.Okay);
            }

            await base.InitializeAsync();
        }

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
                    {"UserId", userName}
                });

            IsBusy = false;
        }

        /// <summary>
        /// Takip edilen kullanıcı listesine yönlendirir
        /// </summary>
        private void ExecuteGotoFollowingCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PushNavigationPageAsync(nameof(FollowingView), new NavigationParameters
                {
                    {"UserId", userName}
                });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand GotoBackCommand
        {
            get { return new Command(() => GoBackAsync()); }
        }

        public ICommand GotoFollowersCommand
        {
            get { return new Command(() => ExecuteGotoFollowersCommand()); }
        }

        public ICommand GotoFollowingCommand
        {
            get { return new Command(() => ExecuteGotoFollowingCommand()); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (IsInitialized)
                return;

            if (parameters.ContainsKey("UserName")) userName = parameters.GetValue<string>("UserName");

            if (parameters.ContainsKey("IsVisibleBackArrow")) IsVisibleBackArrow = parameters.GetValue<bool>("IsVisibleBackArrow");

            InitializeCommand.Execute(null);
            IsInitialized = true;
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            // tabs sayfalarında ilk açılışta tüm dataları çekmesin sayfaya gelirse çeksin diye base methodu ezdik
        }

        #endregion Navigation
    }
}