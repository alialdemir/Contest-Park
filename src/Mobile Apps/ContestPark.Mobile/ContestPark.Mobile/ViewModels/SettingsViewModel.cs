using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SettingsViewModel : ViewModelBase<MenuItemList>
    {
        #region Private variables

        private readonly IIdentityService _identityService;

        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public SettingsViewModel(INavigationService navigationService,
                                IIdentityService identityService,
                                ISettingsService settingsService) : base(navigationService)
        {
            Title = ContestParkResources.Settings;
            _identityService = identityService;
            _settingsService = settingsService;
        }

        #endregion Constructors

        #region Properties

        public bool IsExit { get; set; }

        #endregion Properties

        #region Methods

        protected override Task InitializeAsync()
        {
            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AppSettings)
                                {
                                    new Models.MenuItem.MenuItem {
                                        PageName = nameof(LanguageView),
                                        Icon = "fas-globe",
                                        Title = ContestParkResources.Language,
                                        MenuType = Enums.MenuTypes.Icon
                                    },
                                    new Models.MenuItem.MenuItem {
                                        PageName = "SoundEffects",
                                        Icon = "fas-volume-up",
                                        Title = ContestParkResources.Sounds,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = _settingsService.IsSoundEffectActive
                                    },
                                },

                new MenuItemList(ContestParkResources.AccountSettings)
                                    {
                                    new Models.MenuItem.MenuItem {
                                        PageName = nameof(AccountSettingsView),
                                        Icon = "fas-user-circle",
                                        Title = ContestParkResources.Account,
                                        MenuType = Enums.MenuTypes.Icon
                                    },
                                    new Models.MenuItem.MenuItem {
                                        PageName = nameof(BlockingView),
                                        Icon = "fas-exclamation-circle",
                                        Title = ContestParkResources.Blocking,
                                        MenuType = Enums.MenuTypes.Icon
                                    },
                                    new Models.MenuItem.MenuItem {
                                        PageName = "PrivateProfile",
                                        Icon = "fas-unlock-alt",
                                        Title = ContestParkResources.PrivateProfile,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = _settingsService.IsPrivatePrice
                                    },
                            },

                new MenuItemList(ContestParkResources.Other)
                                {
                                    new Models.MenuItem.MenuItem {
                                        PageName ="Exit",
                                        Icon = "fas-sign-out-alt",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.None
                                    },
                                },
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="name"></param>
        private async Task ExecutePushPageCommand(string name)
        {
            if (IsExit || IsBusy)
                return;

            IsBusy = true;

            if (name == "Exit")
            {
                await _identityService.Unauthorized();
                await PushNavigationPageAsync($"app:///{nameof(SignInView)}?appModuleRefresh=OnInitialized");
            }
            else if (name == "SoundEffects")
            {
                await _settingsService.AddOrUpdateValue(!_settingsService.IsSoundEffectActive, nameof(_settingsService.IsSoundEffectActive));
            }
            else if (name == "PrivateProfile")
            {
                await _settingsService.AddOrUpdateValue(!_settingsService.IsPrivatePrice, nameof(_settingsService.IsPrivatePrice));
            }
            else if (!string.IsNullOrEmpty(name))
            {
                await PushNavigationPageAsync(name);
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _pushPageCommand;

        public ICommand PushPageCommand
        {
            get { return _pushPageCommand ?? (_pushPageCommand = new Command<string>(async (pageName) => await ExecutePushPageCommand(pageName))); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsExit = false;
            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            IsExit = true;
            base.OnNavigatedFrom(parameters);
        }

        #endregion Navigation
    }
}