using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Media;
using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Profile;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Media;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System.Collections.Generic;
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
        private readonly IAnalyticsService _analyticsService;
        private readonly IEventAggregator _eventAggregator;
        public readonly ISettingsService _settingsService;
        private string _userName;
        private SubscriptionToken _changeUserInfoEventSubscriptionToken;
        private SubscriptionToken _postRefreshEventSubscriptionToken;
        private SubscriptionToken _changedFollowCountEventSubscriptionToken;

        #endregion Private variables

        #region Constructor

        public ProfileViewModel(
            INavigationService navigationService,
            IPageDialogService dialogService,
            IIdentityService identityService,
            IPostService postService,
            IPopupNavigation popupNavigation,
            IAnalyticsService analyticsService,
            IEventAggregator eventAggregator,
            ISettingsService settingsService,
            IBlockingService blockingService,
            IFollowService followService,
            IMediaService mediaService

            ) : base(navigationService, dialogService, popupNavigation)
        {
            Title = ContestParkResources.Profile;

            _identityService = identityService;
            _postService = postService;
            _analyticsService = analyticsService;
            _eventAggregator = eventAggregator;
            NavigationService = navigationService;
            _settingsService = settingsService;
            _blockingService = blockingService;
            _followService = followService;
            _mediaService = mediaService;
        }

        #endregion Constructor

        #region Properties

        private bool _isMeProfile = false;
        private bool _isVisibleBackArrow = true;
        private ProfileInfoModel _profileInfo;

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
            get { return _isVisibleBackArrow; }
            set
            {
                _isVisibleBackArrow = value;
                RaisePropertyChanged(() => IsVisibleBackArrow);
            }
        }

        public INavigationService NavigationService { get; }

        /// <summary>
        /// Kullanıcı profil bilgileri
        /// </summary>
        public ProfileInfoModel ProfileInfo
        {
            get { return _profileInfo; }
            set
            {
                _profileInfo = value;
                RaisePropertyChanged(() => ProfileInfo);
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("UserName")) _userName = parameters.GetValue<string>("UserName");

            if (parameters.ContainsKey("IsVisibleBackArrow")) IsVisibleBackArrow = parameters.GetValue<bool>("IsVisibleBackArrow");

            GetProfileInfoByUserNameCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Profil eventleri unsubscribe yapıldı
        /// </summary>
        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            if (_changeUserInfoEventSubscriptionToken != null)
            {
                _eventAggregator
                    .GetEvent<ChangeUserInfoEvent>()
                    .Unsubscribe(_changeUserInfoEventSubscriptionToken);
            }

            if (_postRefreshEventSubscriptionToken != null)
            {
                _eventAggregator
                    .GetEvent<PostRefreshEvent>()
                        .Unsubscribe(_postRefreshEventSubscriptionToken);
            }

            if (_changedFollowCountEventSubscriptionToken != null)
            {
                _eventAggregator
                        .GetEvent<ChangedFollowCountEvent>()
                        .Unsubscribe(_changedFollowCountEventSubscriptionToken);
            }

            return base.GoBackAsync(parameters, useModalNavigation: false);
        }

        /// <summary>
        /// Profil, kapak veya kullanıcı adı değişince profili günceller
        /// </summary>
        private void EventSubscription()
        {
            if (_settingsService.CurrentUser.UserId == ProfileInfo.UserId)// eğer kendi profili ise profil, kapak veya kullanıcı bilgileri güncellenirse
            {
                _changeUserInfoEventSubscriptionToken = _eventAggregator
                                                  .GetEvent<ChangeUserInfoEvent>()
                                                  .Subscribe((userInfo) =>
                                                  {
                                                      if (userInfo == null)
                                                          return;

                                                      if (!string.IsNullOrEmpty(userInfo.FullName))
                                                          ProfileInfo.FullName = userInfo.FullName;

                                                      if (!string.IsNullOrEmpty(userInfo.ProfilePicturePath) && userInfo.ProfilePicturePath != DefaultImages.DefaultProfilePicture)
                                                          ProfileInfo.ProfilePicturePath = userInfo.ProfilePicturePath;

                                                      if (!string.IsNullOrEmpty(userInfo.CoverPicturePath) && userInfo.CoverPicturePath != DefaultImages.DefaultCoverPicture)
                                                          ProfileInfo.CoverPicture = userInfo.CoverPicturePath;
                                                  });

                _postRefreshEventSubscriptionToken = _eventAggregator
                                                                .GetEvent<PostRefreshEvent>()
                                                                .Subscribe(() => RefreshCommand.Execute(null));
            }

            _changedFollowCountEventSubscriptionToken = _eventAggregator
                                                                .GetEvent<ChangedFollowCountEvent>()
                                                                .Subscribe(async (userId) =>
                                                                {
                                                                    if (_settingsService.CurrentUser.UserId == ProfileInfo.UserId)
                                                                    {
                                                                        var currentUserProfileInfo = await _identityService.GetProfileInfoByUserName(_settingsService.CurrentUser.UserName);
                                                                        if (currentUserProfileInfo != null)
                                                                            ProfileInfo = currentUserProfileInfo;
                                                                    }

                                                                    if (userId != ProfileInfo.UserId)
                                                                        return;

                                                                    var profileInfo = await _identityService.GetProfileInfoByUserName(_userName);
                                                                    if (profileInfo != null)
                                                                        ProfileInfo = profileInfo;
                                                                });
        }

        /// <summary>
        /// Mesaj detayına git
        /// </summary>
        /// <param name="receiverUserId">alıcının kullanıcı id</param>
        public void GotoChatDetail()
        {
            if (IsBusy || ProfileInfo == null)
                return;

            IsBusy = true;

            NavigateToAsync<ChatDetailView>(new NavigationParameters
                {
                    { "UserName", _userName},
                    { "FullName", ProfileInfo.FullName},
                    { "SenderUserId", ProfileInfo.UserId},
                    {"SenderProfilePicturePath", ProfileInfo.ProfilePicturePath }
                });

            _analyticsService.SendEvent("Profil", "Sohbet", _userName);

            IsBusy = false;
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
            else
            {
                _analyticsService.SendEvent("Profil",
                                            ProfileInfo.IsBlocked ? "Engel Kaldır" : "Engelle",
                                            _userName);
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
                GotoPhotoModalPage(pictureType);
                IsBusy = false;

                return;
            }

            string selected = await DisplayActionSheetAsync(ContestParkResources.ChooseAnAction,
                                                            ContestParkResources.Cancel,
                                                            null,
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
                    case "Profile":
                        await _identityService.ChangeProfilePictureAsync(media);

                        _analyticsService.SendEvent("Profil", "Profil Resmi Değiştir", media.AnalyticsEventLabel);

                        break;

                    case "Cover":
                        await _identityService.ChangeCoverPictureAsync(media);

                        _analyticsService.SendEvent("Profil", "Kapak Resmi Değiştir", media.AnalyticsEventLabel);

                        break;
                }
            }
            else if (string.Equals(selected, ContestParkResources.ShowImage))
            {
                GotoPhotoModalPage(pictureType);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Takip et takipten çıkar
        /// </summary>
        private async Task ExecuteFollowProcessCommand()
        {
            if (IsBusy || ProfileInfo == null || string.IsNullOrEmpty(ProfileInfo.UserId))
                return;

            IsBusy = true;

            ProfileInfo.IsFollowing = !ProfileInfo.IsFollowing;

            bool isSuccesss = await (ProfileInfo.IsFollowing == true ?
                  _followService.FollowUpAsync(ProfileInfo.UserId) :
                  _followService.UnFollowAsync(ProfileInfo.UserId));

            if (!isSuccesss)
            {
                ProfileInfo.IsFollowing = !ProfileInfo.IsFollowing;

                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }
            else
            {
                _analyticsService.SendEvent("Profil",
                                            ProfileInfo.IsFollowing ? "Takipten Çıkart" : "Takip Et",
                                            _userName);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Takip edenler listesine yönlendirir
        /// </summary>
        private void ExecuteGotoFollowersCommand()
        {
            if (IsBusy || (ProfileInfo == null || string.IsNullOrEmpty(ProfileInfo.UserId) || ProfileInfo.FollowersCount == "0"))
                return;

            IsBusy = true;

            NavigateToAsync<FollowersView>(new NavigationParameters
                {
                    {"UserId", ProfileInfo.UserId}
                });

            _analyticsService.SendEvent("Profil", "Takip Edenler", _userName);

            IsBusy = false;
        }

        /// <summary>
        /// Takip edilen kullanıcı listesine yönlendirir
        /// </summary>
        private void ExecuteGotoFollowingCommand()
        {
            if (IsBusy || (ProfileInfo != null && string.IsNullOrEmpty(ProfileInfo.UserId) || ProfileInfo.FollowUpCount == "0"))
                return;

            IsBusy = true;

            NavigateToAsync<FollowingView>(new NavigationParameters
                {
                    {"UserId", ProfileInfo.UserId}
                });

            _analyticsService.SendEvent("Profil", "Takip Edilenler", _userName);

            IsBusy = false;
        }

        /// <summary>
        /// engelle, şikayet et gibi menüyü açar
        /// </summary>
        private async Task ExecuteInfoCommand()
        {
            string selected = await DisplayActionSheetAsync(ContestParkResources.ChooseAnAction,
                                                            ContestParkResources.Cancel,
                                                               null,
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
        private void ExecutePlayDuelCommand()
        {
            if (IsBusy || ProfileInfo == null || string.IsNullOrEmpty(ProfileInfo.UserId))
                return;

            IsBusy = true;

            NavigateToPopupAsync<SelectSubCategoryView>(new NavigationParameters()
            {
                { "OpponentUserId", ProfileInfo.UserId }
            });

            _analyticsService.SendEvent("Profil", "Düello Daveti", _userName);

            IsBusy = false;
        }

        /// <summary>
        /// modalName göre modal açar
        /// </summary>
        /// <param name="modalName">Açılacak modalda gösterilecek resim</param>
        private void GotoPhotoModalPage(string modalName)
        {
            if (ProfileInfo == null || string.IsNullOrEmpty(modalName))
                return;

            List<PictureModel> pictures = new List<PictureModel>();

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
                NavigateToPopupAsync<PhotoModalView>(new NavigationParameters
                {
                    { "Pictures", pictures }
                });
            }
        }

        /// <summary>
        /// Kullanıcı profilini getirir
        /// </summary>
        private async Task ExecuteGetProfileInfoByUserNameCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            if (ProfileInfo == null)
            {
                var profileInfo = await _identityService.GetProfileInfoByUserName(_userName);
                if (profileInfo == null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlertAsync("",
                                                            ContestParkResources.UserNotFound,
                                                            ContestParkResources.Okay);
                    });

                    IsBusy = false;

                    return;
                }

                ProfileInfo = profileInfo;

                IsMeProfile = _settingsService.CurrentUser.UserId == ProfileInfo.UserId;

                EventSubscription();
            }

            if (IsMeProfile || !ProfileInfo.IsPrivateProfile)
            {
                ServiceModel = await _postService.GetPostsByUserIdAsync(ProfileInfo.UserId, ServiceModel);
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand GetProfileInfoByUserNameCommand => new CommandAsync(ExecuteGetProfileInfoByUserNameCommand);

        public ICommand ChangePhotoCommand => new CommandAsync<string>(ChangePhotoAsync);

        public ICommand FollowProcessCommand
        {
            get { return new CommandAsync(ExecuteFollowProcessCommand); }
        }

        public ICommand GotChatDetailCommand
        {
            get { return new Command(GotoChatDetail); }
        }

        public ICommand GotoFollowersCommand
        {
            get { return new Command(ExecuteGotoFollowersCommand); }
        }

        public ICommand GotoFollowingCommand
        {
            get { return new Command(ExecuteGotoFollowingCommand); }
        }

        public ICommand InfoCommand
        {
            get { return new CommandAsync(ExecuteInfoCommand); }
        }

        public ICommand PlayDuelCommand
        {
            get { return new Command(ExecutePlayDuelCommand); }
        }

        public ICommand RemoveEvents
        {
            get
            {
                return new Command(() =>
               {
                   if (_changeUserInfoEventSubscriptionToken != null)
                   {
                       _eventAggregator
                           .GetEvent<ChangeUserInfoEvent>()
                           .Unsubscribe(_changeUserInfoEventSubscriptionToken);
                   }
               });
            }
        }

        #endregion Commands
    }
}
