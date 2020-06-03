using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IBalanceService _cpService;
        private readonly IIdentityService _identityService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public AppShellViewModel(INavigationService navigationService,
                                 IEventAggregator eventAggregator,
                                 IPageDialogService pageDialogService,
                                 IBalanceService cpService,
                                 IIdentityService identityService,
                                 IAnalyticsService analyticsService,
                                 ISettingsService settingsService) : base(navigationService, pageDialogService)
        {
            _eventAggregator = eventAggregator;
            _cpService = cpService;
            _identityService = identityService;
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

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            SetUserInfoCommand.Execute(null);
            SetUserGoldCommand.Execute(null);

            ListenerEventsCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        private async Task SetUserGoldAsync()
        {
            var balance = await _cpService.GetBalanceAsync();
            if (balance != null)
            {
                Balance = balance;
            }
        }

        /// <summary>
        /// Sol menüdeki kullanıcı bilgilerini getirir ve appcenter user id ekler
        /// </summary>
        private async Task ExecuteSetUserInfoCommand()
        {
            UserInfoModel currentUser = await _identityService.GetUserInfo();
            if (currentUser != null)
            {
                _settingsService.RefreshCurrentUser(currentUser);

                FullName = currentUser.FullName;
                ProfilePicture = currentUser.ProfilePicturePath;
                CoverPicture = currentUser.CoverPicturePath;

                AppCenter.SetUserId(currentUser.UserId);
            }
        }

        /// <summary>
        /// Kullanıcı bilgileri değiştir ve bakiye değişme eventlerini dinler
        /// </summary>
        private void ExecuteListenerEventsCommand()
        {
            _eventAggregator
                .GetEvent<ChangeUserInfoEvent>()
                .Subscribe((userInfo) =>
                {
                    if (userInfo == null)
                        return;

                    if (!string.IsNullOrEmpty(userInfo.FullName))
                        FullName = userInfo.FullName;

                    if (!string.IsNullOrEmpty(userInfo.ProfilePicturePath) && userInfo.ProfilePicturePath != DefaultImages.DefaultProfilePicture)
                        ProfilePicture = userInfo.ProfilePicturePath;

                    if (!string.IsNullOrEmpty(userInfo.CoverPicturePath) && userInfo.CoverPicturePath != DefaultImages.DefaultCoverPicture)
                        CoverPicture = userInfo.CoverPicturePath;
                });

            _eventAggregator
                .GetEvent<GoldUpdatedEvent>()
                .Subscribe(() => SetUserGoldCommand.Execute(null));
        }

        /// <summary>
        /// Sol menü item tıklamaları
        /// </summary>
        /// <param name="name">Yönlendirilecek view adı veya url</param>
        private void ExecuteMenuItemClickCommand(string name)
        {
            if (IsBusy || string.IsNullOrEmpty(name))
                return;

            IsBusy = true;

            if (_settingsService.CurrentUser.UserId == "34873f81-dfee-4d78-bc17-97d9b9bb-bot"
                && (name.StartsWith("https://contestpark.com/balancecode.html") || name.StartsWith("BalanceCodeView")))
            {
                DisplayAlertAsync(string.Empty,
                                  ContestParkResources.ComingSoon,
                                  ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            if (name.IsUrl())
            {
                _analyticsService.SendEvent("Sol Menü", "Link", name);

                if (name.EndsWith("balancecode.html"))
                    Launcher.TryOpenAsync(new Uri($"{name}?q={_settingsService.AuthAccessToken}"));
                else
                    Shell.Current.GoToAsync($"{nameof(BrowserView)}?Link={name}");

                Shell.Current.FlyoutIsPresented = false;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                _analyticsService.SendEvent("Sol Menü", "Menü link", name);

                Shell.Current.FlyoutIsPresented = false;

                Shell.Current.GoToAsync(name);
            }

            IsBusy = false;
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

        private ICommand SetUserGoldCommand => new CommandAsync(SetUserGoldAsync);

        private ICommand SetUserInfoCommand => new CommandAsync(ExecuteSetUserInfoCommand);

        private ICommand ListenerEventsCommand => new Command(ExecuteListenerEventsCommand);
        public ICommand MenuItemClickCommand => new Command<string>(ExecuteMenuItemClickCommand);

        #endregion Commands
    }
}
