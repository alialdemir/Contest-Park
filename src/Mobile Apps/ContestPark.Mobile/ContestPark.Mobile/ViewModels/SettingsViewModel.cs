using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Identity;
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
                                    new TextMenuItem {
                                        CommandParameter = nameof(LanguageView),
                                        Icon = "settings_language.svg",
                                        Title = ContestParkResources.Language,
                                        MenuType = Enums.MenuTypes.Label,
                                        //IconColor ="#0D0D0D",
                                        SingleTap = pushPageCommand
                                    },
                                    new Models.MenuItem.SwitchMenuItem {
                                        Icon = "settings_sound.svg",
                                        Title = ContestParkResources.Sounds,
                                        MenuType = Enums.MenuTypes.Switch,
                                        //IconColor ="#0D0D0D",
                                        IsToggled = _settingsService.IsSoundEffectActive,
                                        SingleTap =  new Command(()=> ChangeSoundAsync()),
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },

                new MenuItemList(ContestParkResources.AccountSettings)
                                    {
                                    new TextMenuItem {
                                        CommandParameter = nameof(AccountSettingsView),
                                        Icon = "settings_account.svg",
                                        Title = ContestParkResources.EditProfile,
                                        MenuType = Enums.MenuTypes.Label,
                                        //IconColor ="#0D0D0D",
                                        SingleTap = pushPageCommand
                                    },
                                    new TextMenuItem {
                                        CommandParameter = nameof(BlockingView),
                                        Icon = "settings_blocked.svg",
                                        Title = ContestParkResources.Blocking,
                                        MenuType = Enums.MenuTypes.Label,
                                        //IconColor ="#0D0D0D",
                                        SingleTap = pushPageCommand
                                    },
                                    new SwitchMenuItem {
                                        Icon = "settings_private_profile.svg",
                                        Title = ContestParkResources.PrivateProfile,
                                        MenuType = Enums.MenuTypes.Switch,
                                        //IconColor ="#0D0D0D",
                                        IsToggled = _settingsService.CurrentUser.IsPrivateProfile,
                                        SingleTap = new Command( async()=> await  ChangePrivateProfileAsync()),
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                            },

                new MenuItemList(ContestParkResources.Other)
                                {
                                    new TextMenuItem {
                                        Icon = "settings_log_out.svg",
                                        Title = ContestParkResources.LogOut,
                                        MenuType = Enums.MenuTypes.Label,
                                        //IconColor ="#0D0D0D",
                                        SingleTap = new Command(async()=> await ExitAppAsync()),
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },
            });

            IsBusy = false;

            return base.InitializeAsync();
        }

        /// <summary>
        /// Profili private/public olarak değiştirir
        /// </summary>
        private async Task ChangePrivateProfileAsync()
        {
            _settingsService.CurrentUser.IsPrivateProfile = !_settingsService.CurrentUser.IsPrivateProfile;

            bool isSuccess = await _identityService.UpdateUserInfoAsync(new UpdateUserInfoModel
            {
                FullName = _settingsService.CurrentUser.FullName,
                UserName = _settingsService.CurrentUser.UserName,
                IsPrivateProfile = _settingsService.CurrentUser.IsPrivateProfile
            });
            if (isSuccess)
            {
                _settingsService.RefreshCurrentUser(_settingsService.CurrentUser);
            }
            else
            {
                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }
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

            await PushNavigationPageAsync($"app:///{nameof(PhoneNumberView)}?appModuleRefresh=OnInitialized");
        }

        #endregion Methods
    }
}
