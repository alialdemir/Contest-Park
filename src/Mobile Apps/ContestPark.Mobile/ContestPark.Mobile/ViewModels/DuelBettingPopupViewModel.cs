using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Balance;
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

        private readonly IBalanceService _cpService;

        #endregion Private variables

        #region Constructor

        public DuelBettingPopupViewModel(INavigationService navigationService,
                                         IBalanceService cpService,
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
        private BalanceModel _balance = new BalanceModel();

        public ObservableRangeCollection<BetModel> Bets { get; set; } = new ObservableRangeCollection<BetModel>
                            {    new BetModel
                                {
                                    Image = "prizeicon1.png",
                                    BalanceType = BalanceTypes.Gold,
                                    Description = "Buraya herhangi bir açıklama gelebilir.",
                                    Title = ContestParkResources.Beginner,
                                    Prize = 40 * 2,
                                    EntryFee = 40,
                                    CurrentIndex = 1,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon2.png",
                                    BalanceType = BalanceTypes.Gold,
                                    Description = "Buraya herhangi bir açıklama gelebilir.",
                                    Title = ContestParkResources.Novice,
                                    Prize = 300 * 2,
                                    EntryFee = 300,
                                    CurrentIndex = 2,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon3.png",
                                    BalanceType = BalanceTypes.Gold,
                                    Description = "Buraya herhangi bir açıklama gelebilir.",
                                    Title = ContestParkResources.Advanced,
                                    Prize = 1000 * 2,
                                    EntryFee = 1000,
                                    CurrentIndex = 3,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon4.png",
                                    BalanceType = BalanceTypes.Gold,
                                    Description = "Buraya herhangi bir açıklama gelebilir.",
                                    Title = ContestParkResources.Expert,
                                    Prize = 3000 * 2,
                                    EntryFee = 3000,
                                    CurrentIndex = 4,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon5.png",
                                    BalanceType = BalanceTypes.Gold,
                                    Description = "Buraya herhangi bir açıklama gelebilir.",
                                    Title = ContestParkResources.Master,
                                    Prize = 4600 * 2,
                                    EntryFee = 4600,
                                    CurrentIndex = 5,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon6.png",
                                    BalanceType = BalanceTypes.Gold,
                                    Description = "Buraya herhangi bir açıklama gelebilir.",
                                    Title = ContestParkResources.Genius,
                                    Prize = 10000 * 2,
                                    EntryFee = 10000,
                                    CurrentIndex = 6,
                                },
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

        public BalanceModel Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                RaisePropertyChanged(() => Balance);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Balance = await _cpService.GetTotalCpByUserIdAsync();

            AddFreeLoader();

            IsBusy = false;
        }

        /// <summary>
        /// Eğer kullanıcının altın miktarı sıfır ise altınsız oynaması için bahis miktari eklenir
        /// </summary>
        private void AddFreeLoader()
        {
            // TODO: eğer hiç altını yoksa video izle oyna özelliği eklenmeli
            if (Balance.Gold == 0)// Altını hiç yoksa 0 altınla oynayabilir
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
        private async Task ExecuteDuelStartCommandAsync(decimal bet)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (bet <= Balance.Gold)
            {
                await PushPopupPageAsync(new DuelStartingPopupView()
                {
                    SelectedSubCategory = SelectedSubCategory,
                    Bet = bet,
                    OpponentUserId = OpponentUserId,
                    BalanceType = Enums.BalanceTypes.Gold,// şimdilik gold verdim
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
