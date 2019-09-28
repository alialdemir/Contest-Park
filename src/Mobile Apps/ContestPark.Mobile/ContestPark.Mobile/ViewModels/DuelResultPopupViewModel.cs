using ContestPark.Mobile.Components.DuelResultSocialMedia;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Models.Duel.DuelResultSocialMedia;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
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
        private readonly IDuelService _duelService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public DuelResultPopupViewModel(
            IPopupNavigation popupNavigation,
            IEventAggregator eventAggregator,
            IAudioService audioService,
            ISettingsService settingsService,
            IDuelService duelService
            ) : base(popupNavigation: popupNavigation)
        {
            _eventAggregator = eventAggregator;
            _audioService = audioService;
            _settingsService = settingsService;
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
            DuelResult = await _duelService.DuelResult(DuelId);

            ProfilePictureBorderColorCommand?.Execute(null);

            if (DuelResult != null && _settingsService.IsSoundEffectActive && DuelResult.IsShowFireworks)
                _audioService.Play(Audio.Fireworks, true);

            await base.InitializeAsync();
        }

        /// <summary>
        /// Başka rakip bul
        /// </summary>
        private async Task ExecuteFindOpponentCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            await RemoveFirstPopupAsync();

            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SelectedSubCategory = new SelectedSubCategoryModel
                {
                    SubcategoryId = DuelResult.SubCategoryId,
                    SubcategoryName = DuelResult.SubCategoryName,
                    SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
                }
            });

            IsBusy = false;
        }

        /// <summary>
        /// Mesaj detaya git
        /// </summary>
        private async Task ExecuteGotoChatCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            await RemoveFirstPopupAsync();

            bool isFounder = DuelResult.IsFounder;
            string userName = isFounder ? DuelResult.OpponentUserName : DuelResult.FounderUserName;
            string fullName = isFounder ? DuelResult.OpponentFullName : DuelResult.FounderFullName;
            string userId = isFounder ? DuelResult.OpponentUserId : DuelResult.FounderUserId;
            string profilePicturePath = isFounder ? DuelResult.OpponentProfilePicturePath : DuelResult.FounderProfilePicturePath;

            _eventAggregator
                .GetEvent<TabPageNavigationEvent>()
                .Publish(new Models.PageNavigation.PageNavigation(nameof(ChatDetailView))
                {
                    Parameters = new NavigationParameters
                                {
                                    { "UserName", userName},
                                    { "FullName", fullName},
                                    { "SenderUserId", userId},
                                    {"SenderProfilePicturePath", profilePicturePath }
                                }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            await RemoveFirstPopupAsync();

            _eventAggregator
                .GetEvent<TabPageNavigationEvent>()
                .Publish(new Models.PageNavigation.PageNavigation(nameof(ProfileView))
                {
                    Parameters = new NavigationParameters
                                {
                                        {"UserName", userName }
                                }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Rövanş düello başlatır
        /// </summary>
        private async Task ExecuteRevengeCommand()
        {
            if (IsBusy || DuelResult == null)
                return;

            IsBusy = true;

            await RemoveFirstPopupAsync();
            // TODO: burada karşı rakipbe rövanş yapmak ister misiniz diye sorması lazım
            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SelectedSubCategory = new SelectedSubCategoryModel
                {
                    SubcategoryId = DuelResult.SubCategoryId,
                    SubcategoryName = DuelResult.SubCategoryName,
                    SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
                },
                OpponentUserId = DuelResult.IsFounder ? DuelResult.OpponentUserId : DuelResult.FounderUserId
            });

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
                    Gold = DuelResult.Gold
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

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await RemoveFirstPopupAsync();

                    _eventAggregator.GetEvent<PostRefreshEvent>();
                });
            }
        }

        public ICommand FindOpponentCommand { get { return new Command(async () => await ExecuteFindOpponentCommand()); } }
        public ICommand GotoChatCommand { get { return new Command(async () => await ExecuteGotoChatCommand()); } }
        public ICommand GotoProfilePageCommand { get { return new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)); } }
        public ICommand RevengeCommand { get { return new Command(async () => await ExecuteRevengeCommand()); } }
        public ICommand ShareCommand { get { return new Command(() => ExecuteShareCommand()); } }

        public ICommand ProfilePictureBorderColorCommand { get; set; }

        #endregion Commands
    }
}
