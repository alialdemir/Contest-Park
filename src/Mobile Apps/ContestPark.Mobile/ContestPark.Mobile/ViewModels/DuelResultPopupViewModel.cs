using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Components.DuelResultSocialMedia;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Models.Duel.DuelResultSocialMedia;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Plugin.StoreReview;
using Plugin.StoreReview.Abstractions;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class DuelResultPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IAudioService _audioService;
        private readonly ICacheService _cacheService;
        private readonly IDuelService _duelService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IAdMobService _adMobService;
        private readonly ISettingsService _settingsService;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Constructors

        public DuelResultPopupViewModel(
            IEventAggregator eventAggregator,
            IAdMobService adMobService,
            IAudioService audioService,
            ICacheService cacheService,
            IPopupNavigation popupNavigation,
            ISettingsService settingsService,
            INavigationService navigationService,
            IPageDialogService dialogService,
            IAnalyticsService analyticsService,
            IDuelService duelService
            ) : base(navigationService, dialogService, popupNavigation)
        {
            _eventAggregator = eventAggregator;
            _adMobService = adMobService;
            _audioService = audioService;
            _cacheService = cacheService;
            _settingsService = settingsService;
            _analyticsService = analyticsService;
            _duelService = duelService;
        }

        #endregion Constructors

        #region Properties

        private int _duelId;
        private DuelResultModel _duelResult;

        public DuelResultModel DuelResult
        {
            get { return _duelResult; }
            set
            {
                _duelResult = value;
                RaisePropertyChanged(() => DuelResult);
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            _adMobService.ShowInterstitial();// Düello sonucuna gelen kullanıcılara reklam gösterildi

            parameters.TryGetValue("DuelId", out _duelId);

            InitDuelResultCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Düello sonucunu getirir
        /// </summary>
        private async Task ExecuteInitDuelResultCommand()
        {
            if (_duelId <= 0)
                return;

            _settingsService.RemovePendingDuelId(_duelId);

            DuelResult = await _duelService.DuelResult(_duelId);

            ProfilePictureBorderColorCommand?.Execute(null);

            if (DuelResult != null && _settingsService.IsSoundEffectActive && DuelResult.IsShowFireworks)
                _audioService.Play(AudioTypes.Fireworks, true);
        }

        /// <summary>
        /// Başka rakip bul
        /// </summary>
        private void ExecuteFindOpponentCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            GotoBackCommand.Execute(true);

            var selectedSubCategory = new SelectedSubCategoryModel
            {
                SubcategoryId = DuelResult.SubCategoryId,
                SubCategoryName = DuelResult.SubCategoryName,
                SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
            };

            NavigateToPopupAsync<DuelBettingPopupView>(new NavigationParameters
            {
                { "SelectedSubCategory",selectedSubCategory }
            });

            _analyticsService.SendEvent("Düello Sonucu", "Rakip Bul", DuelResult.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Mesaj detaya git
        /// </summary>
        private void ExecuteGotoChatCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            GotoBackCommand.Execute(true);

            bool isFounder = DuelResult.IsFounder;
            string userName = isFounder ? DuelResult.OpponentUserName : DuelResult.FounderUserName;
            string fullName = isFounder ? DuelResult.OpponentFullName : DuelResult.FounderFullName;
            string userId = isFounder ? DuelResult.OpponentUserId : DuelResult.FounderUserId;
            string profilePicturePath = isFounder ? DuelResult.OpponentProfilePicturePath : DuelResult.FounderProfilePicturePath;

            _eventAggregator
                .GetEvent<TabPageNavigationEvent>()
                .Publish(new PageNavigation(nameof(ChatDetailView))
                {
                    Parameters = new NavigationParameters
                                {
                                    { "UserName", userName},
                                    { "FullName", fullName},
                                    { "SenderUserId", userId},
                                    {"SenderProfilePicturePath", profilePicturePath }
                                }
                });

            _analyticsService.SendEvent("Düello Sonucu", "Sohbet", DuelResult.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            GotoBackCommand.Execute(true);

            _eventAggregator
                .GetEvent<TabPageNavigationEvent>()
                .Publish(new PageNavigation(nameof(ProfileView))
                {
                    Parameters = new NavigationParameters
                                {
                                        {"UserName", userName }
                                }
                });

            _analyticsService.SendEvent("Düello Sonucu", "Profile Git", DuelResult.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Rövanş düello başlatır
        /// </summary>
        private void ExecuteRevengeCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            GotoBackCommand.Execute(true);

            var selectedSubCategory = new SelectedSubCategoryModel
            {
                SubcategoryId = DuelResult.SubCategoryId,
                SubCategoryName = DuelResult.SubCategoryName,
                SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
                OpponentUserId = DuelResult.IsFounder ? DuelResult.OpponentUserId : DuelResult.FounderUserId
            };

            NavigateToPopupAsync<DuelBettingPopupView>(new NavigationParameters
            {
                { "SelectedSubCategory",selectedSubCategory },
            });

            _analyticsService.SendEvent("Düello Sonucu", "Rövanş", DuelResult.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Düello sonucunu sosyal medyada paylaş
        /// </summary>
        private async void ExecuteShareCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            IConvertUIToImage convertUIToImage = Xamarin.Forms.DependencyService.Get<IConvertUIToImage>();
            if (convertUIToImage == null)
            {
                IsBusy = false;
                return;
            }

            string path = convertUIToImage.GetImagePathByPage(new DuelResultSocialMediaView()
            {
                ViewModel = new DuelResultSocialMediaModel
                {
                    FounderColor = DuelResult.FounderColor,
                    OpponentColor = DuelResult.OpponentColor,
                    FounderProfilePicturePath = DuelResult.FounderProfilePicturePath,
                    OpponentProfilePicturePath = DuelResult.OpponentProfilePicturePath,
                    SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
                    FounderFullName = DuelResult.FounderFullName,
                    OpponentFullName = DuelResult.OpponentFullName,
                    SubCategoryName = DuelResult.SubCategoryName,
                    Date = DateTime.Now.ToString("MMMM dd, yyyy"),
                    FounderScore = DuelResult.FounderScore,
                    OpponentScore = DuelResult.OpponentScore,
                    Gold = DuelResult.Gold,
                    BalanceType = DuelResult.BalanceType,
                    IsShowFireworks = DuelResult.IsShowFireworks
                }
            });

            if (string.IsNullOrEmpty(path))
            {
                IsBusy = false;
                return;
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = Title,
                File = new ShareFile(path)
            });

            _analyticsService.SendEvent("Düello Sonucu", "Paylaş", DuelResult.SubCategoryName);

            IsBusy = false;
        }

        public override async Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            if (_settingsService.IsSoundEffectActive)
                _audioService?.Stop();

            _eventAggregator
                .GetEvent<PostRefreshEvent>()
                .Publish();

            if (DuelResult != null && DuelResult.WinnerOrLoseText != ContestParkResources.Tie)
            {
                _eventAggregator
                    .GetEvent<GoldUpdatedEvent>()
                    .Publish();
            }

            bool isStoreReview = await _cacheService.Get<bool>("IsStoreReview");
            if (DuelResult != null && DuelResult.IsShowFireworks && !isStoreReview && CrossStoreReview.IsSupported)
            {
                await RequestReview();
            }
            else if (!(await _cacheService.Get<bool>("SpecialOffer")))
            {
                await NavigateToPopupAsync<SpecialOfferPopupView>();

                _cacheService.Add("SpecialOffer", true, TimeSpan.FromDays(1));
            }

            await base.RemoveFirstPopupAsync<DuelResultPopupView>();
        }

        /// <summary>
        /// Store üzerinden uygulamaya yıldız ver uyarısını gösterir
        /// </summary>
        private async Task RequestReview()
        {
            IStoreReview storeReview = CrossStoreReview.Current;

            string message = Device.RuntimePlatform == Device.Android
                                           ? ContestParkResources.WouldYouLikeToRateTheGameOnGooglePlay
                                           : ContestParkResources.WouldYouLikeToRateTheGameOnAppStore;

            bool isOk = await DisplayAlertAsync(string.Empty,
                                              message,
                                              ContestParkResources.Okay,
                                              ContestParkResources.Cancel);
            if (isOk)
            {
                if (Device.RuntimePlatform == Device.Android)
                    storeReview.OpenStoreReviewPage(AppInfo.PackageName);
                else if (Device.RuntimePlatform == Device.iOS)
                    storeReview.RequestReview();

                _cacheService.Add("IsStoreReview", true, TimeSpan.FromDays(30));
            }
            else
            {
                _cacheService.Add("IsStoreReview", false, TimeSpan.FromHours(1));
            }
        }

        #endregion Methods

        #region Commands

        private ICommand InitDuelResultCommand => new CommandAsync(ExecuteInitDuelResultCommand);

        public ICommand FindOpponentCommand { get { return new Command(ExecuteFindOpponentCommand); } }
        public ICommand GotoChatCommand { get { return new Command(ExecuteGotoChatCommand); } }
        public ICommand GotoProfilePageCommand { get { return new Command<string>(ExecuteGotoProfilePageCommand); } }
        public ICommand RevengeCommand { get { return new Command(ExecuteRevengeCommand); } }
        public ICommand ShareCommand { get { return new Command(ExecuteShareCommand); } }

        public ICommand ProfilePictureBorderColorCommand { get; set; }

        #endregion Commands
    }
}
