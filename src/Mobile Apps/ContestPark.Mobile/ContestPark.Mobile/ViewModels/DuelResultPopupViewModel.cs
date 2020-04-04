using ContestPark.Mobile.Components.DuelResultSocialMedia;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Models.Duel.DuelResultSocialMedia;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
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
            ISettingsService settingsService,
            INavigationService navigationService,
            IAnalyticsService analyticsService,
            IDuelService duelService
            ) : base(navigationService: navigationService)
        {
            _eventAggregator = eventAggregator;
            _adMobService = adMobService;
            _audioService = audioService;
            _settingsService = settingsService;
            _analyticsService = analyticsService;
            _duelService = duelService;
        }

        #endregion Constructors

        #region Properties

        public int DuelId { get; set; }
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

        protected override async Task InitializeAsync()
        {
            _adMobService.ShowInterstitial();// Düello sonucuna gelen kullanıcılara reklam gösterildi

            DuelResult = await _duelService.DuelResult(DuelId);

            ProfilePictureBorderColorCommand?.Execute(null);

            if (DuelResult != null && _settingsService.IsSoundEffectActive && DuelResult.IsShowFireworks)
                _audioService.Play(Audio.Fireworks, true);

            await base.InitializeAsync();
        }

        /// <summary>
        /// Başka rakip bul
        /// </summary>
        private void ExecuteFindOpponentCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            ClosePopupCommand.Execute(null);

            var selectedSubCategory = new SelectedSubCategoryModel
            {
                SubcategoryId = DuelResult.SubCategoryId,
                SubCategoryName = DuelResult.SubCategoryName,
                SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
            };

            PushModalAsync(nameof(DuelBettingPopupView), new NavigationParameters
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

            ClosePopupCommand.Execute(null);

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

            ClosePopupCommand.Execute(null);

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

            ClosePopupCommand.Execute(null);

            var selectedSubCategory = new SelectedSubCategoryModel
            {
                SubcategoryId = DuelResult.SubCategoryId,
                SubCategoryName = DuelResult.SubCategoryName,
                SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
                OpponentUserId = DuelResult.IsFounder ? DuelResult.OpponentUserId : DuelResult.FounderUserId
            };

            PushModalAsync(nameof(DuelBettingPopupView), new NavigationParameters
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

            IConvertUIToImage convertUIToImage = DependencyService.Get<IConvertUIToImage>();
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

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand
        {
            get
            {
                return new Command(() =>
              {
                  if (_settingsService.IsSoundEffectActive)
                      _audioService?.Stop();

                  GotoBackCommand.Execute(null);

                  _eventAggregator.GetEvent<PostRefreshEvent>();

                  _eventAggregator.GetEvent<GoldUpdatedEvent>();
              });
            }
        }

        public ICommand FindOpponentCommand { get { return new Command(() => ExecuteFindOpponentCommand()); } }
        public ICommand GotoChatCommand { get { return new Command(() => ExecuteGotoChatCommand()); } }
        public ICommand GotoProfilePageCommand { get { return new Command<string>((userName) => ExecuteGotoProfilePageCommand(userName)); } }
        public ICommand RevengeCommand { get { return new Command(() => ExecuteRevengeCommand()); } }
        public ICommand ShareCommand { get { return new Command(() => ExecuteShareCommand()); } }

        public ICommand ProfilePictureBorderColorCommand { get; set; }

        #endregion Commands
    }
}
