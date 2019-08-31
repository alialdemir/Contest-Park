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
    public class IbanNoViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IBalanceService _balanceService;
        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructor

        public IbanNoViewModel(IBalanceService balanceService,
                               IEventAggregator eventAggregator,
                               IPageDialogService pageDialogService,
                               INavigationService navigationService) : base(navigationService: navigationService,
                                                                            dialogService: pageDialogService)
        {
            Title = ContestParkResources.ConvertToCash;
            _balanceService = balanceService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        private IbanNoModel _ibanNo = new IbanNoModel();

        public IbanNoModel IbanNo
        {
            get { return _ibanNo; }
            set
            {
                _ibanNo = value;
                RaisePropertyChanged(() => IbanNo);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Iban no girme ekranına yönlendirir
        /// </summary>
        private async Task ExecuteBalanceCommand()
        {
            if (string.IsNullOrEmpty(IbanNo.IbanNo) || string.IsNullOrEmpty(IbanNo.FullName))
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.RequiredFields,
                                        ContestParkResources.Okay);
                return;
            }

            BalanceModel balance = await _balanceService.GetBalanceAsync();

            decimal minBalanceAmount = 100.00m;
            if (balance.Money < minBalanceAmount)
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.YouMustearnAminimumOfToWithdrawYourwinnings,
                                        ContestParkResources.Okay);
                return;
            }

            bool isSuccess = await _balanceService.GetBalanceRequest(IbanNo);
            if (isSuccess)
            {
                _eventAggregator
                     .GetEvent<GoldUpdatedEvent>()
                     .Publish();

                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.ARequestToTransferYourEarningsToYourAccountHasBeenCreated,
                                        ContestParkResources.Okay);

                await GoBackAsync();
            }
        }

        #endregion Methods

        #region Commands

        public ICommand BalanceCommand
        {
            get { return new Command(async () => await ExecuteBalanceCommand()); }
        }

        #endregion Commands
    }
}
