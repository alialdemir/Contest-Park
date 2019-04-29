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
    /// <summary>
    /// Defines the <see cref="SettingsViewModel"/>
    /// </summary>
    public class SettingsViewModel : ViewModelBase<MenuItemList>
    {
        #region Fields

        /// <summary>
        /// Defines the _identityService
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Defines the _settingsService
        /// </summary>
        private readonly ISettingsService _settingsService;

        #endregion Fields

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

        /// <summary>
        /// The InitializeAsync
        /// </summary>
        protected override Task InitializeAsync()
        {
            if (IsBusy)
                return Task.CompletedTask;

            IsBusy = true;

            ICommand pushPageCommand = new Command<string>(async (pageName) => await ExecutePushPageCommand(pageName));

            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AppSettings)
                                {
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = nameof(LanguageView),
                                        Icon = "fas-globe",
                                        Title = ContestParkResources.Language,
                                        MenuType = Enums.MenuTypes.Icon,
                                        SingleTap = pushPageCommand
                                    },
                                    new Models.MenuItem.MenuItem {
                                        Icon = "fas-volume-up",
                                        Title = ContestParkResources.Sounds,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = _settingsService.IsSoundEffectActive,
                                        SingleTap =  new Command(()=> ChangeSoundAsync())
                                    },
                                },

                new MenuItemList(ContestParkResources.AccountSettings)
                                    {
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = nameof(AccountSettingsView),
                                        Icon = "fas-user-circle",
                                        Title = ContestParkResources.EditProfile,
                                        MenuType = Enums.MenuTypes.Icon,
                                        SingleTap = pushPageCommand
                                    },
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = nameof(BlockingView),
                                        Icon = "fas-exclamation-circle",
                                        Title = ContestParkResources.Blocking,
                                        MenuType = Enums.MenuTypes.Icon,
                                        SingleTap = pushPageCommand
                                    },
                                    new Models.MenuItem.MenuItem {
                                        Icon = "fas-unlock-alt",
                                        Title = ContestParkResources.PrivateProfile,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = _settingsService.IsPrivatePrice,
                                        SingleTap = new Command(()=>  ChangePrivateProfileAsync())
                                    },
                            },

                new MenuItemList(ContestParkResources.Other)
                                {
                                    new Models.MenuItem.MenuItem {
                                        Icon = "fas-sign-out-alt",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.Label,
                                        SingleTap = new Command(async()=> await ExitAppAsync())
                                    },
                                },
            });

            IsBusy = false;

            return base.InitializeAsync();
        }

        /// <summary>
        /// Profili private/public olarak değiştirir
        /// </summary>
        private void ChangePrivateProfileAsync()
        {
            _settingsService.AddOrUpdateValue(!_settingsService.IsPrivatePrice, nameof(_settingsService.IsPrivatePrice));
        }

        /// <summary>
        /// Ses effecti aç/kapa
        /// </summary>
        private void ChangeSoundAsync()
        {
            _settingsService.AddOrUpdateValue(!_settingsService.IsSoundEffectActive, nameof(_settingsService.IsSoundEffectActive));
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task ExecutePushPageCommand(string name)
        {
            if (string.IsNullOrEmpty(name) || IsBusy)
                return;

            IsBusy = true;

            await PushNavigationPageAsync(name);

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcı çıkış yap
        /// </summary>
        private async Task ExitAppAsync()
        {
            await _identityService.Unauthorized();

            await PushNavigationPageAsync($"app:///{nameof(SignInView)}?appModuleRefresh=OnInitialized");
        }

        #endregion Methods
    }
}