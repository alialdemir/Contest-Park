using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using MvvmHelpers;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
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
        private readonly ISettingsService _settingsService;

        private string userName;

        #endregion Private variables

        #region Constructor

        public ProfileViewModel(
            INavigationService navigationService,
            IPageDialogService dialogService,
            IIdentityService identityService,
            IPostService postService,
            IPopupNavigation popupNavigation,
            ISettingsService settingsService

            ) : base(navigationService, dialogService, popupNavigation)
        {
            _identityService = identityService;
            _postService = postService;
            NavigationService = navigationService;
            _settingsService = settingsService;
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

        private async Task ChangePhotoAsync(string modalName)
        {
            string selected = await DisplayActionSheetAsync(ContestParkResources.ChooseAnAction,
                                                            ContestParkResources.Cancel,
                                                            "",
                                                            //buttons
                                                            ContestParkResources.ShowImage,
                                                            ContestParkResources.ChooseFromLibrary,
                                                            ContestParkResources.TakeAPhoto);
            if (string.Equals(selected, ContestParkResources.ChooseFromLibrary))
            {
            }
            else if (string.Equals(selected, ContestParkResources.TakeAPhoto))
            {
            }
            else if (string.Equals(selected, ContestParkResources.ShowImage))
            {
                GotoPhotoModalPage(modalName);
            }
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

        /// <summary>
        /// modalName göre modal açar
        /// </summary>
        /// <param name="modalName">Açılacak modalda gösterilecek resim</param>
        private void GotoPhotoModalPage(string modalName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ObservableRangeCollection<PictureModel> pictures = new ObservableRangeCollection<PictureModel>();

            if (modalName == "Profile")
            {
                pictures.Add(new PictureModel
                {
                    PicturePath = ProfileInfo.ProfilePicturePath,
                    PictureType = Enums.PictureTypes.Profile,
                });
            }
            else if (modalName == "Cover")
            {
                pictures.Add(new PictureModel
                {
                    PicturePath = ProfileInfo.CoverPicture,
                    PictureType = Enums.PictureTypes.Cover,
                });
            }

            if (pictures.Count != 0)
            {
                PushPopupPageAsync(new PhotoModalView()
                {
                    Pictures = pictures
                });
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ChangePhotoCommand => new Command<string>(async (listTypes) => await ChangePhotoAsync(listTypes));

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