using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.InviteDuel;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class AcceptDuelInvitationPopupViewModel : ViewModelBase
    {
        #region Private varaibles

        private readonly IEventAggregator _eventAggregator;

        private readonly ISettingsService _settingsService;

        private readonly IDuelService _duelService;

        #endregion Private varaibles

        #region Constructor

        public AcceptDuelInvitationPopupViewModel(INavigationService navigationService,
                                                  IEventAggregator eventAggregator,
                                                  ISettingsService settingsService,
                                                  IPageDialogService pageDialogService,
                                                  IDuelService duelService) : base(navigationService: navigationService, dialogService: pageDialogService)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _duelService = duelService;
        }

        #endregion Constructor

        #region Properties

        private bool IsExit { get; set; }

        private double _timer = 10;

        public double Timer
        {
            get { return _timer; }
            set
            {
                _timer = value;
                RaisePropertyChanged(() => Timer);
            }
        }

        private InviteModel _inviteModel;

        public InviteModel InviteModel
        {
            get
            {
                return _inviteModel;
            }
            set
            {
                _inviteModel = value;
                RaisePropertyChanged(() => InviteModel);
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("InviteModel"))
                InviteModel = parameters.GetValue<InviteModel>("InviteModel");

            TimerCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Düello davetini kabul et
        /// </summary>
        private async Task ExecuteAcceptDuelInviteCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            IsExit = true;

            GotoBackCommand.Execute(true);

            SelectedBetModel selectedBet = new SelectedBetModel
            {
                SubcategoryId = InviteModel.SubCategoryId,
                SubCategoryPicturePath = InviteModel.SubCategoryPicture,
                SubCategoryName = InviteModel.SubCategoryName,
                OpponentUserId = InviteModel.OpponentUserId,
                Bet = InviteModel.Bet,
                BalanceType = InviteModel.BalanceType,
                StandbyMode = DuelStartingPopupViewModel.StandbyModes.Invited,
            };

            await NavigateToPopupAsync<DuelStartingPopupView>(new NavigationParameters
            {
                { "SelectedDuelInfo", selectedBet }
            });

            bool isSuccess = await _duelService.AcceptInviteDuel(new AcceptInviteDuelModel
            {
                BalanceType = InviteModel.BalanceType,
                Bet = InviteModel.EntryFee,
                SubCategoryId = InviteModel.SubCategoryId,
                FounderConnectionId = InviteModel.FounderConnectionId,
                FounderLanguage = InviteModel.FounderLanguage,
                FounderUserId = InviteModel.FounderUserId,
                OpponentConnectionId = _settingsService.SignalRConnectionId,
            });
            if (!isSuccess)
            {
                await GoBackAsync();

                bool isBuy = await DisplayAlertAsync(
                    ContestParkResources.NoGold,
                    ContestParkResources.YouDoNotHaveASufficientAmountOfGoldToOpenThisCategory,
                    ContestParkResources.Buy,
                    ContestParkResources.Cancel);
                if (isBuy)
                {
                    _eventAggregator
                                .GetEvent<TabPageNavigationEvent>()
                                .Publish(new PageNavigation(nameof(ContestStoreView)));
                }
            }

            IsBusy = false;
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            IsExit = true;

            return base.GoBackAsync(parameters, useModalNavigation);
        }

        /// <summary>
        /// Düello davetini kabul etmesi için kalan sürenin geri sayımını yapar
        /// </summary>
        private void ExecuteTimerCommand()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () =>
          {
              if (IsExit)
                  return false;

              Timer -= 0.100;

              if (Timer <= 0)
                  GotoBackCommand.Execute(true);

              return Timer > 0;
          });
        }

        #endregion Methods

        #region Commands

        private ICommand TimerCommand => new Command(ExecuteTimerCommand);

        public ICommand AcceptDuelInviteCommand { get { return new CommandAsync(ExecuteAcceptDuelInviteCommand); } }

        #endregion Commands
    }
}
