﻿using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IBalanceService _cpService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public AppShellViewModel(INavigationService navigationService,
                                 IEventAggregator eventAggregator,
                                 IBalanceService cpService,
                                 IAnalyticsService analyticsService,
                                 ISettingsService settingsService) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _cpService = cpService;
            _analyticsService = analyticsService;
            _settingsService = settingsService;

            InitializeCommand?.Execute(null);
        }

        #endregion Constructors

        #region Property

        private string _fullName;

        private string _profilePicture;

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        private BalanceModel _balance = new BalanceModel();

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;

                RaisePropertyChanged(() => FullName);
            }
        }

        public string ProfilePicture
        {
            get { return _profilePicture; }
            set
            {
                _profilePicture = value;

                RaisePropertyChanged(() => ProfilePicture);
            }
        }

        private string _coverPicture;

        public string CoverPicture
        {
            get { return _coverPicture; }
            set
            {
                _coverPicture = value;
                RaisePropertyChanged(() => CoverPicture);
            }
        }

        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public BalanceModel Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                _balance = value;
                RaisePropertyChanged(() => Balance);
            }
        }

        #endregion Property

        #region Methods

        protected override async Task InitializeAsync()
        {
            UserInfoModel currentUser = await _settingsService.GetUserInfo();
            if (currentUser != null)
            {
                _settingsService.RefreshCurrentUser(currentUser);

                FullName = currentUser.FullName;
                ProfilePicture = currentUser.ProfilePicturePath;
                CoverPicture = currentUser.CoverPicturePath;
            }

            SetUserGoldCommand.Execute(null);

            _eventAggregator
                .GetEvent<ChangeUserInfoEvent>()
                .Subscribe((userInfo) =>
                {
                    if (userInfo != null)
                    {
                        FullName = userInfo.FullName;
                        ProfilePicture = userInfo.ProfilePicturePath;
                    }
                });

            _eventAggregator
                .GetEvent<GoldUpdatedEvent>()
                .Subscribe(() =>
                {
                    SetUserGoldCommand.Execute(null);
                });

            await base.InitializeAsync();
        }

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        /// <returns></returns>
        private async Task SetUserGoldAsync()
        {
            var balance = await _cpService.GetBalanceAsync();
            if (balance != null)
            {
                Balance = balance;
            }
        }

        #endregion Methods

        #region Commands

        /// <summary>
        /// Profil sayfasına yönlendirir
        /// </summary>
        public ICommand GotoProfileViewCommand
        {
            get => new Command(() =>
            {
                if (_eventAggregator != null)
                {
                    _analyticsService.SendEvent("Sol Menü", "Sol menü profil fotoğrafı", _settingsService.CurrentUser.UserName);

                    _eventAggregator.GetEvent<TabChangeEvent>().Publish(Enums.Tabs.Profile);

                    _eventAggregator
                                .GetEvent<MasterDetailPageIsPresentedEvent>()
                                .Publish(false);
                }
            });
        }

        private ICommand SetUserGoldCommand => new Command(async () => await SetUserGoldAsync());

        #endregion Commands
    }
}
