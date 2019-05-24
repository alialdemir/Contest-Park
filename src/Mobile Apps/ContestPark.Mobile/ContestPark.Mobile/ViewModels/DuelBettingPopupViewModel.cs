using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Bet;
using ContestPark.Mobile.Services.Cp;
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
    public class DuelBettingPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ICpService _cpService;

        #endregion Private variables

        #region Constructor

        public DuelBettingPopupViewModel(INavigationService navigationService,
                                         ICpService cpService,
                                         IPageDialogService pageDialogService,
                                         IPopupNavigation popupNavigation
            ) : base(navigationService, pageDialogService, popupNavigation)
        {
            _cpService = cpService;
        }

        #endregion Constructor

        #region Properties

        private int _selectedIndex = 0;

        /// <summary>
        /// Kullanıcının altın miktarını tutar
        /// </summary>
        private int _userCp = 0;

        public ObservableRangeCollection<BetModel> Bets { get; set; } = new ObservableRangeCollection<BetModel>
                            {
                                new BetModel { Title =ContestParkResources.Beginner, Prize = 40  * 2, EntryFee = 20},
                                new BetModel { Title = ContestParkResources.Novice, Prize = 300  * 2, EntryFee = 150},
                                new BetModel { Title = ContestParkResources.Advanced, Prize = 1000  * 2, EntryFee = 500},
                                new BetModel { Title = ContestParkResources.Expert, Prize = 3000  * 2, EntryFee = 1500},
                                new BetModel { Title = ContestParkResources.Master, Prize = 4600  * 2, EntryFee = 2300},
                                new BetModel { Title = ContestParkResources.Genius, Prize = 10000  * 2, EntryFee = 5000},
                            };

        public string OpponentUserId { get; set; }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        public SelectedSubCategoryModel SelectedSubCategory { get; } = new SelectedSubCategoryModel();

        public int UserCp
        {
            get { return _userCp; }
            set
            {
                _userCp = value;
                RaisePropertyChanged(() => UserCp);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserCp = await _cpService.GetTotalCpByUserIdAsync();

            AddFreeLoader();

            IsBusy = false;
        }

        /// <summary>
        /// Eğer kullanıcının altın miktarı sıfır ise altınsız oynaması için bahis miktari eklenir
        /// </summary>
        private void AddFreeLoader()
        {
            // TODO: eğer hiç altını yoksa video izle oyna özelliği eklenmeli
            if (UserCp == 0)// Altını hiç yoksa 0 altınla oynayabilir
            {
                Bets.Insert(0, new BetModel
                {
                    Title = ContestParkResources.Freeloader,
                    EntryFee = 0,
                    Prize = 0
                });

                SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Seçilen altın miktarı kadar altını varsa düello başlatır yoksa mesaj verir
        /// </summary>
        /// <param name="bet">Seçilen bahis miktarı</param>
        private async Task ExecuteDuelStartCommandAsync(int bet)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (bet <= UserCp)
            {
                await PushPopupPageAsync(new DuelStartingPopupView()
                {
                    SelectedSubCategory = SelectedSubCategory,
                    Bet = bet,
                    OpponentUserId = OpponentUserId,
                    StandbyMode = string.IsNullOrEmpty(OpponentUserId) ? DuelStartingPopupViewModel.StandbyModes.On : DuelStartingPopupViewModel.StandbyModes.Off,
                });

                ClosePopupCommand.Execute(null);
            }
            else
            {
                await DisplayAlertAsync("",
                   ContestParkResources.YouDontHaveEnoughGoldToPlayYouCanBuyGoldFromTheContestStore,
                   ContestParkResources.Okay,
                   ContestParkResources.Cancel);
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        public ICommand DuelStartCommand => new Command<int>(async (bet) => await ExecuteDuelStartCommandAsync(bet));

        #endregion Commands
    }
}