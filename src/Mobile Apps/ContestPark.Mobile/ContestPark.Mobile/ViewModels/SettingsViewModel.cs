using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Analytics;
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

        private readonly IAnalyticsService _analyticsService;

        /// <summary>
        /// Defines the _settingsService
        /// </summary>
        private readonly ISettingsService _settingsService;

        #endregion Fields

        #region Constructors

        public SettingsViewModel(INavigationService navigationService,
                                 IIdentityService identityService,
                                 IAnalyticsService analyticsService,
                                 ISettingsService settingsService) : base(navigationService)
        {
            Title = ContestParkResources.Settings;
            _identityService = identityService;
            _analyticsService = analyticsService;
            _settingsService = settingsService;
        }

        #endregion Constructors

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            LoadSettingsCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Profili private/public olarak değiştirir
        /// </summary>
        private async Task ChangePrivateProfileAsync()
        {
            _analyticsService.SendEvent("Ayarlar", "Tıklama", "Private Profil");

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
            _analyticsService.SendEvent("Ayarlar", "Tıklama", "Ses Ayarı");

            _settingsService.IsSoundEffectActive = !_settingsService.IsSoundEffectActive;
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The <see cref="Task"/></returns>
        private void ExecutePushPageCommand<TViewModel>() where TViewModel : ContentPage
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string name = typeof(TViewModel).Name;
            _analyticsService.SendEvent("Ayarlar", "Tıklama", name);

            NavigateToAsync<TViewModel>();

            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcı çıkış yap
        /// </summary>
        private async Task ExitAppAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            await _identityService.Unauthorized();

            await NavigateToInitialized<PhoneNumberView>();

            UserDialogs.Instance.HideLoading();

            IsBusy = false;
        }

        private void ExecuteLoadSettingsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = new ServiceModel<MenuItemList>
            {
                Items = new List<MenuItemList>()
                        {
                            new MenuItemList(ContestParkResources.AppSettings)
                                            {
                                                new TextMenuItem {
                                                    Icon =  ContestParkApp.Current.Resources["LanguageSettings"].ToString(),
                                                    Title = ContestParkResources.Language,
                                                    MenuType = MenuTypes.Label,
                                                    SingleTap = new Command(() =>  ExecutePushPageCommand<LanguageView>())
                                                },
                                                new SwitchMenuItem {
                                                    Icon =  ContestParkApp.Current.Resources["SoundSettings"].ToString(),
                                                    Title = ContestParkResources.Sounds,
                                                    MenuType = MenuTypes.Switch,
                                                    IsToggled = _settingsService.IsSoundEffectActive,
                                                    SingleTap =  new Command(()=> ChangeSoundAsync()),
                                                    CornerRadius = new CornerRadius(0,0,8,8)
                                                },
                                            },

                            new MenuItemList(ContestParkResources.AccountSettings)
                                                {
                                                new TextMenuItem {
                                                    Icon = ContestParkApp.Current.Resources["AccountSettings"].ToString(),
                                                    Title = ContestParkResources.EditProfile,
                                                    MenuType = MenuTypes.Label,
                                                    SingleTap = new Command(() =>  ExecutePushPageCommand<AccountSettingsView>())
                                                },
                                                new TextMenuItem {
                                                    Icon = ContestParkApp.Current.Resources["BlockedSettings"].ToString(),
                                                    Title = ContestParkResources.Blocking,
                                                    MenuType = MenuTypes.Label,
                                                    SingleTap = new Command(() =>  ExecutePushPageCommand<BlockingView>())
                                                },
                                                new SwitchMenuItem {
                                                    Icon = ContestParkApp.Current.Resources["PrivateProfileSettings"].ToString(),
                                                    Title = ContestParkResources.PrivateProfile,
                                                    MenuType = MenuTypes.Switch,
                                                    IsToggled = _settingsService.CurrentUser.IsPrivateProfile,
                                                    SingleTap = new Command( async()=> await  ChangePrivateProfileAsync()),
                                                    CornerRadius = new CornerRadius(0,0,8,8)
                                                },
                                        },

                            new MenuItemList(ContestParkResources.Other)
                                            {
                                                new TextMenuItem {
                                                    Icon = ContestParkApp.Current.Resources["Exit"].ToString(),
                                                    Title = ContestParkResources.LogOut,
                                                    MenuType = MenuTypes.Label,
                                                    SingleTap = new Command(async()=> await ExitAppAsync()),
                                                    CornerRadius = new CornerRadius(0,0,8,8)
                                                },
                                            },
                        }
            };

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand LoadSettingsCommand => new Command(ExecuteLoadSettingsCommand);

        #endregion Commands
    }
}
