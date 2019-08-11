using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Media;
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

        private readonly IBlockingService _blockingService;
        private readonly IFollowService _followService;
        private readonly IIdentityService _identityService;
        private readonly IMediaService _mediaService;
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
            ISettingsService settingsService,
            IBlockingService blockingService,
            IFollowService followService,
            IMediaService mediaService

            ) : base(navigationService, dialogService, popupNavigation)
        {
            _identityService = identityService;
            _postService = postService;
            NavigationService = navigationService;
            _settingsService = settingsService;
            _blockingService = blockingService;
            _followService = followService;
            _mediaService = mediaService;
            Title = ContestParkResources.Profile;
        }

        #endregion Constructor

        #region Properties

        private bool _isMeProfile = false;
        private bool isVisibleBackArrow = false;
        private ProfileInfoModel profileInfo;

        /// <summary>
        /// Kendi profilindemi onu döndürür true ise kendi profili false ise başkasının profili
        /// </summary>
        public bool IsMeProfile
        {
            get
            {
                return _isMeProfile;
            }

            set
            {
                _isMeProfile = value;
                RaisePropertyChanged(() => IsMeProfile);
            }
        }

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

        /// <summary>
        /// Mesaj detayına git
        /// </summary>
        /// <param name="receiverUserId">alıcının kullanıcı id</param>
        public void GotoChatDetail()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PushNavigationPageAsync(nameof(ChatDetailView), new NavigationParameters
                {
                    { "UserName", userName},
                    { "FullName", ProfileInfo.FullName},
                    { "SenderUserId", ProfileInfo.UserId},
                    {"SenderProfilePicturePath", ProfileInfo.ProfilePicturePath }
                });

            IsBusy = false;
        }

        protected override async Task InitializeAsync()
        {
            var profileInfo = await _identityService.GetProfileInfoByUserName(userName);

            if (profileInfo != null)
            {
                ProfileInfo = profileInfo;

                IsMeProfile = _settingsService.CurrentUser.UserId == ProfileInfo.UserId;

                if (IsMeProfile || !ProfileInfo.IsPrivateProfile)
                {
                    ServiceModel = await _postService.GetPostsByUserIdAsync(ProfileInfo.UserId, ServiceModel);
                }
            }
            else
            {
                await DisplayAlertAsync("",
                    ContestParkResources.UserNotFound,
                    ContestParkResources.Okay);
            }

            IsBusy = false;

            await base.InitializeAsync();
        }

        /// <summary>
        /// Engelle engeli kaldır işlemi
        /// </summary>
        private async Task BlockProcess()
        {
            if (IsBusy || string.IsNullOrEmpty(ProfileInfo.UserId))
                return;

            IsBusy = true;

            // TODO: burada tersini aldığımız için aşağıda yanlış işlem yapıyor olabilir kontrol edilmesi lazım
            ProfileInfo.IsBlocked = !ProfileInfo.IsBlocked;

            bool isSuccesss = await (ProfileInfo.IsBlocked == true ?
                  _blockingService.Block(ProfileInfo.UserId) :
                  _blockingService.UnBlock(ProfileInfo.UserId));

            if (!isSuccesss)
            {
                ProfileInfo.IsBlocked = !ProfileInfo.IsBlocked;

                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Profil yada kapak resmine tıklayınca
        /// eğer kendi profili ise resmi değiştir görüntüle gibi seçenek çıkar
        /// başkasının profili ise direk görüntüler
        /// </summary>
        /// <param name="pictureType">Kapak resmi yada profil resmi hangisine tıklanarak geldi</param>
        private async Task ChangePhotoAsync(string pictureType)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!IsMeProfile)
            {
                await GotoPhotoModalPage(pictureType);
                IsBusy = false;

                return;
            }

            string selected = await DisplayActionSheetAsync(ContestParkResources.ChooseAnAction,
                                                            ContestParkResources.Cancel,
                                                            "",
                                                            //buttons
                                                            ContestParkResources.ShowImage,
                                                            ContestParkResources.ChooseFromLibrary,
                                                            ContestParkResources.TakeAPhoto);

            if (string.Equals(selected, ContestParkResources.ChooseFromLibrary) || string.Equals(selected, ContestParkResources.TakeAPhoto))
            {
                MediaModel media = await _mediaService.GetPictureStream(selected);
                if (media == null)
                {
                    IsBusy = false;
                    return;
                }

                switch (pictureType)
                {
                    case "Profile": await _identityService.ChangeCoverPictureAsync(media); break;
                    case "Cover": await _identityService.ChangeProfilePictureAsync(media); break;
                }
            }
            else if (string.Equals(selected, ContestParkResources.ShowImage))
            {
                await GotoPhotoModalPage(pictureType);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Takip et takipten çıkar
        /// </summary>
        private async Task ExecuteFollowProcessCommand()
        {
            if (IsBusy || string.IsNullOrEmpty(ProfileInfo.UserId))
                return;

            IsBusy = true;

            // TODO: burada tersini aldığımız için aşağıda yanlış işlem yapıyor olabilir kontrol edilmesi lazım
            ProfileInfo.IsFollowing = !ProfileInfo.IsFollowing;

            bool isSuccesss = await (ProfileInfo.IsFollowing == true ?
                  _followService.UnFollowAsync(ProfileInfo.UserId) :
                  _followService.FollowUpAsync(ProfileInfo.UserId));

            if (!isSuccesss)
            {
                ProfileInfo.IsFollowing = !ProfileInfo.IsFollowing;

                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }

            IsBusy = false;
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
                    {"UserId", ProfileInfo.UserId}
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
                    {"UserId", ProfileInfo.UserId}
                });

            IsBusy = false;
        }

        /// <summary>
        /// engelle, şikayet et gibi menüyü açar
        /// </summary>
        private async Task ExecuteInfoCommand()
        {
            string selected = await DisplayActionSheetAsync(ContestParkResources.ChooseAnAction,
                                                            ContestParkResources.Cancel,
                                                               "",
                                                           //buttons
                                                           ProfileInfo.IsBlocked ? ContestParkResources.RemoveBlock : ContestParkResources.Block);
            if (string.Equals(selected, ContestParkResources.RemoveBlock) || string.Equals(selected, ContestParkResources.Block))
            {
                await BlockProcess();
            }
        }

        /// <summary>
        /// Düello başlat
        /// </summary>
        private async Task ExecutePlayDuelCommand()
        {
            // TODO: Kategori listesi gelmeli seçilen kategoride düello başlatılmalı

            await DisplayAlertAsync("", "Comingsoon", ContestParkResources.Okay);
        }

        /// <summary>
        /// modalName göre modal açar
        /// </summary>
        /// <param name="modalName">Açılacak modalda gösterilecek resim</param>
        private async Task GotoPhotoModalPage(string modalName)
        {
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
                await PushPopupPageAsync(new PhotoModalView()
                {
                    Pictures = pictures
                });
            }
        }

        #endregion Methods

        #region Commands

        public ICommand ChangePhotoCommand => new Command<string>(async (listTypes) => await ChangePhotoAsync(listTypes));

        public ICommand FollowProcessCommand
        {
            get { return new Command(async () => await ExecuteFollowProcessCommand()); }
        }

        public ICommand GotChatDetailCommand
        {
            get { return new Command(() => GotoChatDetail()); }
        }

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

        public ICommand InfoCommand
        {
            get { return new Command(async () => await ExecuteInfoCommand()); }
        }

        public ICommand PlayDuelCommand
        {
            get { return new Command(async () => await ExecutePlayDuelCommand()); }
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
