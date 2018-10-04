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

        #region Methods

        protected override Task InitializeAsync()
        {
            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AppSettings)
                                {
                                    new Models.MenuItem.MenuItem {
                                        PageName = nameof(LanguageView),
                                        Icon = "ic_public_black_24dp.png",
                                        Title = ContestParkResources.Language,
                                        MenuType = Enums.MenuTypes.Icon
                                    },
                                    new Models.MenuItem.MenuItem {
                                        PageName = "SoundEffects",
                                        Icon = "ic_public_black_24dp.png",
                                        Title = ContestParkResources.Sounds,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = _settingsService.IsSoundEffectActive
                                    },
                                },

                new MenuItemList(ContestParkResources.AccountSettings)
                                    {
                                    new Models.MenuItem.MenuItem {
                                        PageName = nameof(AccountSettingsView),
                                        Icon = "ic_account_circle_black_24dp.png",
                                        Title = ContestParkResources.Account,
                                        MenuType = Enums.MenuTypes.Icon
                                    },
                                    new Models.MenuItem.MenuItem {
                                        PageName = nameof(BlockingView),
                                        Icon = "",
                                        Title = ContestParkResources.Blocking,
                                        MenuType = Enums.MenuTypes.Icon
                                    },
                                    new Models.MenuItem.MenuItem {
                                        PageName = "PrivateProfile",
                                        Icon = "",
                                        Title = ContestParkResources.PrivateProfile,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = _settingsService.IsPrivatePrice
                                    },
                            },

                new MenuItemList(ContestParkResources.Other)
                                {
                                    new Models.MenuItem.MenuItem {
                                        PageName ="Exit",
                                        Icon = "ic_input_black_24dp.png",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.None
                                    },
                                },
            });

            return base.InitializeAsync();
        }

        private void ExecutePushPageCommand(string name)
        {
            if (name == "Exit")
            {
                _identityService.Unauthorized();
                PushNavigationPageAsync("app:///SignInPage?appModuleRefresh=OnInitialized");
            }
            else if (name == "SoundEffects")
            {
                _settingsService.AddOrUpdateValue(!_settingsService.IsSoundEffectActive, nameof(_settingsService.IsSoundEffectActive));
            }
            else if (name == "PrivateProfile")
            {
                _settingsService.AddOrUpdateValue(!_settingsService.IsPrivatePrice, nameof(_settingsService.IsPrivatePrice));
            }
            else if (!string.IsNullOrEmpty(name))
            {
                PushNavigationPageAsync(name);
            }
        }

        #endregion Methods

        #region Commands

        private ICommand _pushPageCommand;

        public ICommand PushPageCommand
        {
            get { return _pushPageCommand ?? (_pushPageCommand = new Command<string>((pageName) => ExecutePushPageCommand(pageName))); }
        }

        #endregion Commands
    }
}