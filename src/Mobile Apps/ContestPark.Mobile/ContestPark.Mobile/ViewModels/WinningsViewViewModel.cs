using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContestPark.Mobile.ViewModels
{
    public class WinningsViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IBalanceService _balanceService;
        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructor

        public WinningsViewModel(IBalanceService balanceService,
                                 INavigationService navigationService,
                                 IEventAggregator eventAggregator,
                                 IPageDialogService dialogService) : base(navigationService: navigationService,
                                                                          dialogService: dialogService)
        {
            Title = ContestParkResources.ConvertToCash;
            _balanceService = balanceService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        private decimal _balance;

        public decimal Balance
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

        /// <summary>
        /// Sayfa açılınca bakiye bilgisini getirir
        /// </summary>
        public override async Task InitializeAsync(INavigationParameters parameters = null)
        {
            await GetBalanceAsync();

            _eventAggregator
                .GetEvent<GoldUpdatedEvent>()
                .Subscribe(async () => await GetBalanceAsync());
        }

        /// <summary>
        /// Sayfada gösterilen bakiye bilgisini yükler
        /// </summary>
        private async Task GetBalanceAsync()
        {
            var balance = await _balanceService.GetBalanceAsync();
            if (balance != null)
            {
                Balance = balance.Money;
            }
        }

        /// <summary>
        /// Iban no girme ekranına yönlendirir
        /// </summary>
        private async Task ExecuteBalanceCommand()
        {
            decimal minBalanceAmount = 20.00m;
            if (Balance < minBalanceAmount)
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.YouMustearnAminimumOfToWithdrawYourwinnings,
                                        ContestParkResources.Okay);
                return;
            }

            await NavigateToAsync<IbanNoView>();
        }

        #endregion Methods

        #region Commands

        public ICommand BalanceCommand
        {
            get { return new CommandAsync(ExecuteBalanceCommand); }
        }

        #endregion Commands
    }
}
