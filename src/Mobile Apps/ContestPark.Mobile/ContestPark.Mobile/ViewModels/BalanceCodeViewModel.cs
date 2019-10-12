using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class BalanceCodeViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IBalanceService _balanceService;
        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructor

        public BalanceCodeViewModel(IBalanceService balanceService,
                               IEventAggregator eventAggregator,
                               IPageDialogService pageDialogService,
                               INavigationService navigationService) : base(navigationService: navigationService,
                                                                            dialogService: pageDialogService)
        {
            _balanceService = balanceService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        private BalanceCodeModel _balanceCode = new BalanceCodeModel();

        public BalanceCodeModel BalanceCode
        {
            get { return _balanceCode; }
            set
            {
                _balanceCode = value;
                RaisePropertyChanged(() => BalanceCode);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Iban no girme ekranına yönlendirir
        /// </summary>
        private async Task ExecuteBalanceCodeCommand()
        {
            if (string.IsNullOrEmpty(BalanceCode.Code))
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.RequiredFields,
                                        ContestParkResources.Okay);
                return;
            }

            bool isSuccess = await _balanceService.BalanceCode(BalanceCode);
            if (isSuccess)
            {
                _eventAggregator
                     .GetEvent<GoldUpdatedEvent>()
                     .Publish();

                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.YourBalanceHasBeenLoaded,
                                        ContestParkResources.Okay);

                await GoBackAsync();
            }
        }

        #endregion Methods

        #region Commands

        public ICommand BalanceCodeCommand
        {
            get { return new Command(async () => await ExecuteBalanceCodeCommand()); }
        }

        #endregion Commands
    }
}
