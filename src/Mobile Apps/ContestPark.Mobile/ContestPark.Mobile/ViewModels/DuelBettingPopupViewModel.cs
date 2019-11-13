using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Bet;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using MvvmHelpers;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class DuelBettingPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IEventAggregator _eventAggregator;

        private readonly IBalanceService _cpService;

        #endregion Private variables

        #region Constructor

        public DuelBettingPopupViewModel(INavigationService navigationService,
                                         IEventAggregator eventAggregator,
                                         IBalanceService cpService,
                                         IPageDialogService pageDialogService,
                                         IPopupNavigation popupNavigation) : base(navigationService, pageDialogService, popupNavigation)
        {
            _eventAggregator = eventAggregator;
            _cpService = cpService;
        }

        #endregion Constructor

        #region Properties

        private int _selectedIndex = 0;
        private BalanceTypes balanceType = BalanceTypes.Gold;

        public BalanceTypes BalanceType
        {
            get { return balanceType; }
            set
            {
                balanceType = value;

                RaisePropertyChanged(() => BalanceType);
            }
        }

        public ObservableRangeCollection<BetModel> Bets { get; set; } = new ObservableRangeCollection<BetModel>();
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

        public SelectedSubCategoryModel SelectedSubCategory { get; set; } = new SelectedSubCategoryModel();

        public BalanceModel Balance { get; set; } = new BalanceModel();

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            InitBets();

            Balance = await _cpService.GetBalanceAsync();

            AddFreeLoader();

            IsBusy = false;
        }

        private void InitBets()
        {
            Bets.Clear();

            if (BalanceType == BalanceTypes.Gold)
            {
                Bets.AddRange(
           new List<BetModel> {
                                new BetModel
                                {
                                    Image = "prizeicon1.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Beginner,
                                    Prize = 80.00m,
                                    EntryFee = 40.00m,
                                    CurrentIndex = 1,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon2.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Novice,
                                    Prize = 600.00m,
                                    EntryFee = 300.00m,
                                    CurrentIndex = 2,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon3.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Advanced,
                                    Prize = 2000.00m,
                                    EntryFee = 1000.00m,
                                    CurrentIndex = 3,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon4.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Expert,
                                    Prize = 6000.00m,
                                    EntryFee = 3000.00m,
                                    CurrentIndex = 4,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon5.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Master,
                                    Prize = 9200.00m,
                                    EntryFee = 4600.00m,
                                    CurrentIndex = 5,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon6.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Genius,
                                    Prize = 20000.00m,
                                    EntryFee = 10000.00m,
                                    CurrentIndex = 6,
                                }
           }
    );
            }
            else if (BalanceType == BalanceTypes.Money)
            {
                Bets.AddRange(
           new List<BetModel> {
                                new BetModel
                                {
                                    Image = "prizeicon1.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Beginner,
                                    Prize = 2.00m,
                                    EntryFee = 1.00m,
                                    CurrentIndex = 1,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon2.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Novice,
                                    Prize = 6.00m,
                                    EntryFee = 3.00m,
                                    CurrentIndex = 2,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon3.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Advanced,
                                    Prize = 8.00m,
                                    EntryFee = 4.00m,
                                    CurrentIndex = 3,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon4.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Expert,
                                    Prize = 10.00m,
                                    EntryFee = 5.00m,
                                    CurrentIndex = 4,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon5.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Master,
                                    Prize = 12.00m,
                                    EntryFee = 6.00m,
                                    CurrentIndex = 5,
                                },
                                new BetModel
                                {
                                    Image = "prizeicon6.png",
                                    BalanceType = BalanceType,
                                    Description = "",
                                    Title = ContestParkResources.Genius,
                                    Prize = 20.00m,
                                    EntryFee = 10.00m,
                                    CurrentIndex = 6,
                                }
           }
    );
            }
        }

        /// <summary>
        /// Eğer kullanıcının altın miktarı sıfır ise altınsız oynaması için bahis miktari eklenir
        /// </summary>
        private void AddFreeLoader()
        {
            // TODO: eğer hiç altını yoksa video izle oyna özelliği eklenmeli
            if (BalanceType == BalanceTypes.Gold && Balance != null && Balance.Gold < Bets?.FirstOrDefault().EntryFee)// Altını hiç yoksa 0 altınla oynayabilir
            {
                Bets.Insert(0, new BetModel
                {
                    Image = "prizeicon1.png",
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

            var balance = await _cpService.GetBalanceAsync();
            if (balance != null && ((BalanceType == BalanceTypes.Gold && bet <= balance.Gold) || (BalanceType == BalanceTypes.Money && bet <= balance.Money)))
            {
                await PushPopupPageAsync(new DuelStartingPopupView()
                {
                    SelectedSubCategory = SelectedSubCategory,
                    Bet = bet,
                    OpponentUserId = OpponentUserId,
                    BalanceType = BalanceType,
                    StandbyMode = string.IsNullOrEmpty(OpponentUserId) ? DuelStartingPopupViewModel.StandbyModes.On : DuelStartingPopupViewModel.StandbyModes.Off,
                });

                ClosePopupCommand.Execute(null);
            }
            else
            {
                string message =
              BalanceType == BalanceTypes.Gold ?
              ContestParkResources.YouDontHaveEnoughGoldToPlayYouCanBuyGoldFromTheContestStore :
              ContestParkResources.YouDontHaveEnoughBalanceToPlayYouMustUploadTheBalanceViaTheContestStore;

                bool isBuy = await DisplayAlertAsync("",
                                            message,
                                            ContestParkResources.Buy,
                                            ContestParkResources.Cancel);

                if (isBuy)
                {
                    await RemoveFirstPopupAsync();

                    _eventAggregator?
                                .GetEvent<TabPageNavigationEvent>()
                                .Publish(new PageNavigation(nameof(ContestStoreView)));
                }
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        public ICommand DuelStartCommand => new Command<decimal>(async (bet) => await ExecuteDuelStartCommandAsync(bet));

        private ICommand _changeBalanceTypeCommand;

        public ICommand ChangeBalanceTypeCommand
        {
            get
            {
                return _changeBalanceTypeCommand ?? (_changeBalanceTypeCommand = new Command<string>((balanceType) =>
                {
                    if (IsBusy || balanceType == ((byte)BalanceType).ToString())
                        return;

                    IsBusy = true;

                    BalanceType = balanceType == "2" ? BalanceTypes.Money : BalanceTypes.Gold;

                    InitBets();

                    IsBusy = false;
                }));
            }
        }

        #endregion Commands
    }
}
