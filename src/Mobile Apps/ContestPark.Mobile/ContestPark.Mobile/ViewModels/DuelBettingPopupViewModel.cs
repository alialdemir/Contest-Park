using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Duel.Bet;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using MvvmHelpers;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class DuelBettingPopupViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ICpService _CpService;

        #endregion Private variables

        #region Constructor

        public DuelBettingPopupViewModel(INavigationService navigationService,
                                         ICpService cpService,
                                         IPopupNavigation popupNavigation) : base(navigationService, popupNavigation: popupNavigation)
        {
            _CpService = cpService;
        }

        #endregion Constructor

        #region Properties

        public ObservableRangeCollection<Bet> Bets { get; set; } = new ObservableRangeCollection<Bet>
                            {
                                new Bet { Title =ContestParkResources.Beginner, Prize = 20  * 2, EntryFee = 20},
                                new Bet { Title = ContestParkResources.Novice, Prize = 150  * 2, EntryFee = 150},
                                new Bet { Title = ContestParkResources.Advanced, Prize = 500  * 2, EntryFee = 500},
                                new Bet { Title = ContestParkResources.Expert, Prize = 1500  * 2, EntryFee = 1500},
                                new Bet { Title = ContestParkResources.Master, Prize = 5000  * 2, EntryFee = 5000},
                                new Bet { Title = ContestParkResources.Genius, Prize = 16000  * 2, EntryFee = 16000},
                            };

        public Int16 SubcategoryId { get; set; }

        public string SubcategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        /// <summary>
        /// Kullanıcının altın miktarını tutar
        /// </summary>
        private int _userCp = 0;

        public int UserCp
        {
            get { return _userCp; }
            set
            {
                _userCp = value;
                RaisePropertyChanged(() => UserCp);
            }
        }

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged(() => SelectedIndex);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserCp = await _CpService.GetTotalCpByUserIdAsync();

            AddFreeLoader();

            IsBusy = false;
        }

        /// <summary>
        /// Eğer kullanıcının altın miktarı sıfır ise altınsız oynaması için bahis miktari eklenir
        /// </summary>
        private void AddFreeLoader()
        {
            if (UserCp == 0)// Altını hiç yoksa 0 altınla oynayabilir
            {
                Bets.Insert(0, new Bet
                {
                    Title = ContestParkResources.Freeloader,
                    EntryFee = 0,
                    Prize = 0
                });

                SelectedIndex = 0;
            }
        }

        private async void ExecuteDuelStartCommand(int bet)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (bet <= UserCp)
            {
                await PushPopupPageAsync(new DuelStartingPopupView()
                {
                    SubcategoryId = SubcategoryId,
                    Bet = bet,
                    StandbyMode = DuelStartingPopupViewModel.StandbyModes.On,
                    SubcategoryName = SubcategoryName,
                    SubCategoryPicturePath = SubCategoryPicturePath
                });

                ClosePopupCommand.Execute(null);
            }
            else
            {
                // TODO: alert altık satın almak ister misiniz
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand DuelStartCommand => new Command<int>(ExecuteDuelStartCommand);

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        #endregion Commands
    }
}