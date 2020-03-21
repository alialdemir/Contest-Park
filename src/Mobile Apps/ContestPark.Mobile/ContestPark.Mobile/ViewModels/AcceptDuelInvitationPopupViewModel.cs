﻿using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Duel.InviteDuel;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
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

        public AcceptDuelInvitationPopupViewModel(IPopupNavigation popupNavigation,
                                                  IEventAggregator eventAggregator,
                                                  ISettingsService settingsService,
                                                  IPageDialogService pageDialogService,
                                                  IDuelService duelService) : base(dialogService: pageDialogService, popupNavigation: popupNavigation)
        {
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            _duelService = duelService;
        }

        #endregion Constructor

        #region Properties

        public bool IsExit { get; set; }

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

        protected override Task InitializeAsync()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () =>
               {
                   if (IsExit)
                       return false;

                   Timer -= 0.100;

                   if (Timer <= 0)
                       ClosePopupCommand.Execute(null);

                   return Timer > 0;
               });

            return base.InitializeAsync();
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

            await RemoveFirstPopupAsync();

            await PushPopupPageAsync(new DuelStartingPopupView
            {
                SelectedSubCategory = new Models.Duel.SelectedSubCategoryModel
                {
                    SubcategoryId = InviteModel.SubCategoryId,
                    SubcategoryName = InviteModel.SubCategoryName,
                    SubCategoryPicturePath = InviteModel.SubCategoryPicture,
                },
                Bet = InviteModel.Bet,
                OpponentUserId = _settingsService.CurrentUser.UserId,
                BalanceType = InviteModel.BalanceType,
                StandbyMode = DuelStartingPopupViewModel.StandbyModes.Invited
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
                await RemoveFirstPopupAsync();

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

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }
        public ICommand AcceptDuelInviteCommand { get { return new Command(async () => await ExecuteAcceptDuelInviteCommand()); } }

        #endregion Commands
    }
}
