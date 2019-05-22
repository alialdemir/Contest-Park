using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class DuelResultPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructors

        public DuelResultPopupViewModel(
            IPopupNavigation popupNavigation,
            IEventAggregator eventAggregator
            ) : base(popupNavigation: popupNavigation)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Properties

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

        protected override Task InitializeAsync()
        {
            DuelResult = new DuelResultModel
            {
                FounderProfilePicturePath = DefaultImages.DefaultProfilePicture,
                FounderUserName = "witcherfearless",
                OpponentProfilePicturePath = DefaultImages.DefaultProfilePicture,
                OpponentUserName = "eliföz",
                SubCategoryName = "Football",
                WinnerOrLoseText = "Sen kazandın",
                FounderFullName = "Ali Aldemir",
                OpponentFullName = "Elif Öz",
                FounderScore = 234,
                OpponentScore = 12,
                FinishBonus = 40,
                VictoryBonus = 30,
                MatchScore = 234,
                OpponentLevel = 1,
                FounderLevel = 7,
            };
            return base.InitializeAsync();
        }

        /// <summary>
        /// Başka rakip bul
        /// </summary>
        private async Task ExecuteFindOpponentCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await RemoveFirstPopupAsync();

            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SubcategoryId = DuelResult.SubCategoryId,
                SubcategoryName = DuelResult.SubCategoryName,
                SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
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
            if (IsBusy)
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
            if (IsBusy)
                return;

            IsBusy = true;

            await RemoveFirstPopupAsync();
            // TODO: burada karşı rakipbe rövanş yapmak ister misiniz diye sorması lazım
            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SubcategoryId = DuelResult.SubCategoryId,
                SubcategoryName = DuelResult.SubCategoryName,
                SubCategoryPicturePath = DuelResult.SubCategoryPicturePath,
                OpponentUserId = DuelResult.IsFounder ? DuelResult.OpponentUserId : DuelResult.FounderUserId
            });

            IsBusy = false;
        }

        /// <summary>
        /// Düello sonucu sosyal medyada paylaş
        /// </summary>
        private Task ExecuteShareCommand()
        {
            return Task.CompletedTask;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }
        public ICommand FindOpponentCommand { get { return new Command(async () => await ExecuteFindOpponentCommand()); } }
        public ICommand GotoChatCommand { get { return new Command(async () => await ExecuteGotoChatCommand()); } }
        public ICommand GotoProfilePageCommand { get { return new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)); } }
        public ICommand RevengeCommand { get { return new Command(async () => await ExecuteRevengeCommand()); } }
        public ICommand ShareCommand { get { return new Command(async () => await ExecuteShareCommand()); } }

        #endregion Commands
    }
}