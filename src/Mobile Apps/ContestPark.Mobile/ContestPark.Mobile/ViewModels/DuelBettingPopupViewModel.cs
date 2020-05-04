using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.Bet;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
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
    public class DuelBettingPopupViewModel : ViewModelBase<BetModel>
    {
        #region Private variables

        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;
        private readonly IBalanceService _cpService;
        private readonly IAdMobService _adMobService;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Constructor

        public DuelBettingPopupViewModel(INavigationService navigationService,
                                         IEventAggregator eventAggregator,
                                         ISettingsService settingsService,
                                         IPopupNavigation popupNavigation,
                                         IBalanceService cpService,
                                         IAdMobService adMobService,
                                         IPageDialogService pageDialogService,
                                         IAnalyticsService analyticsService) : base(navigationService, pageDialogService, popupNavigation)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _cpService = cpService;
            _adMobService = adMobService;
            _analyticsService = analyticsService;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Düello davetleri penceresi gelsin mi
        /// </summary>

        private int _selectedIndex = 0;
        private BalanceTypes _balanceType = BalanceTypes.Gold;

        public BalanceTypes BalanceType
        {
            get { return _balanceType; }
            set
            {
                _balanceType = value;

                RaisePropertyChanged(() => BalanceType);
            }
        }

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

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("SelectedSubCategory"))
                SelectedSubCategory = parameters.GetValue<SelectedSubCategoryModel>("SelectedSubCategory");

            InitBetsCommand.Execute(null);
        }

        /// <summary>
        /// Bakiye çekip bahisleri yükler
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteInitBetsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Balance = await _cpService.GetBalanceAsync();

            InitBets();

            LastPlayedBetCommand.Execute(null);

            IsBusy = false;
        }

        private void InitBets()
        {
            Items.Clear();

            if (BalanceType == BalanceTypes.Gold)
            {
                BetModel freeBet = AddFreeLoader();
                if (freeBet != null)
                    Items.Add(freeBet);

                Items.AddRange(
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
                Items.AddRange(
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
        private BetModel AddFreeLoader()
        {
            decimal minEntryFee = 40.00m;
            if (BalanceType == BalanceTypes.Gold && Balance != null && Balance.Gold < minEntryFee)// Altını hiç yoksa 0 altınla oynayabilir
            {
                _adMobService.OnRewardedVideoAdClosed += OnRewardedVideoAdClosed;

                return GetFreeBet();
            }

            return null;
        }

        private BetModel GetFreeBet()
        {
            return new BetModel
            {
                Image = "prizeicon1.png",
                Title = ContestParkResources.Freeloader,
                BalanceType = BalanceTypes.Gold,
                Description = ContestParkResources.AdvertiseWatchPlayGames,
                EntryFee = 0,
                Prize = 80.00m,
                CurrentIndex = 1
            };
        }

        /// <summary>
        /// Reklam izle oyna başla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRewardedVideoAdClosed(object sender, System.EventArgs e)
        {
            _adMobService.OnRewardedVideoAdClosed -= OnRewardedVideoAdClosed;

            _analyticsService.SendEvent("Düello", "Oyna", "Video İzle Oyna");

            await ExecuteDuelStartCommandAsync(GetFreeBet(), true);
        }

        /// <summary>
        /// Seçilen altın miktarı kadar altını varsa düello başlatır yoksa mesaj verir
        /// </summary>
        /// <param name="bet">Seçilen bahis miktarı</param>
        private async Task ExecuteDuelStartCommandAsync(BetModel bet, bool isRewarded = false)
        {
            if (IsBusy || bet == null)
                return;

            IsBusy = true;

            if (bet.EntryFee == 0 && !isRewarded)
            {
                _adMobService.ShowOrLoadRewardedVideo();

                IsBusy = false;

                return;
            }

            BetModel appropriateBet = AppropriateBet(bet.EntryFee);
            if (appropriateBet != null && bet.EntryFee != 0 && bet.EntryFee != appropriateBet.EntryFee)
            {
                await DisplayAlertAsync(string.Empty,
                    string.Format(ContestParkResources.YouAnPlaywithLessThanOrEqualToTenTimesTheEntryFeeFromYourMinimumBalanceWeAdhereToTheOption, appropriateBet.Title),
                    ContestParkResources.Okay);

                SelectedIndex = appropriateBet.CurrentIndex - 1;

                IsBusy = false;

                return;
            }

            if (Balance != null && ((BalanceType == BalanceTypes.Gold && bet.EntryFee <= Balance.Gold) || (BalanceType == BalanceTypes.Money && bet.EntryFee <= Balance.Money)))
            {
                _analyticsService.SendEvent("Düello", "Oyna", "Success");

                _settingsService.LastSelectedBet = bet;// en son oynan bakiye tipi kayıt edildi

                PushDuelStartingPopupPageAsync(bet.EntryFee);
            }
            else
            {
                await NoGoldDisplayAlertAsync();
            }

            IsBusy = false;
        }

        private BetModel AppropriateBet(decimal bet)
        {
            decimal minBet = (decimal)(bet * 10);

            int lastCurrentIndex = Items.LastOrDefault().CurrentIndex;

            if (BalanceType == BalanceTypes.Money)
            {
                if (Balance.Money <= minBet)
                {
                    return Items.FirstOrDefault(x => x.EntryFee == bet);
                }

                return Items
                        .Where(x => Balance.Money <= (x.EntryFee * 10) || x.CurrentIndex == lastCurrentIndex)
                        .FirstOrDefault();
            }
            else if (BalanceType == BalanceTypes.Gold)
            {
                if (Balance.Gold <= minBet)
                {
                    return Items.FirstOrDefault(x => x.EntryFee == bet);
                }

                return Items
                        .Where(x => Balance.Gold <= (x.EntryFee * 10) || x.CurrentIndex == lastCurrentIndex)
                        .FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Düello ekranına yönlendirir
        /// </summary>
        /// <param name="bet">Bahis miktarı</param>
        private void PushDuelStartingPopupPageAsync(decimal bet)
        {
            SelectedBetModel selectedBet = new SelectedBetModel
            {
                SubcategoryId = SelectedSubCategory.SubcategoryId,
                SubCategoryPicturePath = SelectedSubCategory.SubCategoryPicturePath,
                SubCategoryName = SelectedSubCategory.SubCategoryName,
                OpponentUserId = SelectedSubCategory.OpponentUserId,
                Bet = bet,
                BalanceType = _balanceType,
                StandbyMode = string.IsNullOrEmpty(SelectedSubCategory.OpponentUserId) ? DuelStartingPopupViewModel.StandbyModes.On : DuelStartingPopupViewModel.StandbyModes.Off,
            };

            NavigateToPopupAsync<DuelStartingPopupView>(new NavigationParameters
            {
                { "SelectedDuelInfo", selectedBet }
            });

            RemoveFirstPopupAsync<DuelBettingPopupView>();
        }

        /// <summary>
        /// Altın/Para miktarınız yeterli değil contest store üzerinden yükleyin mesajını gösterir
        /// </summary>
        private async Task NoGoldDisplayAlertAsync()
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
                GotoBackCommand.Execute(true);

                _analyticsService.SendEvent("Düello", "Oyna", "Contest store");

                _eventAggregator?
                            .GetEvent<TabPageNavigationEvent>()
                            .Publish(new PageNavigation(nameof(ContestStoreView)));
            }
        }

        private void ExecuteChangeBalanceTypeCommand(string balanceType)
        {
            if (IsBusy || balanceType == ((byte)BalanceType).ToString())
                return;

            IsBusy = true;

            BalanceType = balanceType == "2" ? BalanceTypes.Money : BalanceTypes.Gold;

            InitBets();

            IsBusy = false;
        }

        /// <summary>
        /// En son oynadığı bakiye miktarını seçili hale getirir
        /// </summary>
        private void ExecuteLastPlayedBetCommand()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                BetModel lastSelectedBet = _settingsService.LastSelectedBet;
                if (lastSelectedBet != null)// En son oynadığı bakiye tipini seçili olarak getiriyoruz
                    BalanceType = lastSelectedBet.BalanceType;

                if (lastSelectedBet != null)// En son oynadığı bahisi seçili olarak getiriyoruz
                {
                    BetModel appropriateBet = AppropriateBet(0);// Oyuncunun bakiyesine en uygun bahis seçeği
                    if (appropriateBet != null && appropriateBet.EntryFee == lastSelectedBet.EntryFee)
                    {
                        SelectedIndex = lastSelectedBet.CurrentIndex - 1;
                    }
                    else if (appropriateBet != null)
                    {
                        SelectedIndex = appropriateBet.CurrentIndex - 1;
                    }
                }
            });
        }

        #endregion Methods

        #region Commands

        private ICommand InitBetsCommand => new CommandAsync(ExecuteInitBetsCommand);

        private ICommand LastPlayedBetCommand => new Command(ExecuteLastPlayedBetCommand);

        public ICommand DuelStartCommand => new Command<BetModel>(async (bet) => await ExecuteDuelStartCommandAsync(bet));

        private ICommand _changeBalanceTypeCommand;

        public ICommand ChangeBalanceTypeCommand
        {
            get
            {
                return _changeBalanceTypeCommand ?? (_changeBalanceTypeCommand = new Command<string>(ExecuteChangeBalanceTypeCommand));
            }
        }

        #endregion Commands
    }
}
