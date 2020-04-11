using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Icons;
using ContestPark.Mobile.Models.Identity;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        /// <summary>
        /// The InitializeAsync
        /// </summary>
        protected override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (IsBusy)
                return Task.CompletedTask;

            IsBusy = true;

            ServiceModel.Items = new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.AppSettings)
                                {
                                    new TextMenuItem {
                                        Icon = ContestParkIcon.LanguageSettings,
                                        Title = ContestParkResources.Language,
                                        MenuType = MenuTypes.Label,
                                        SingleTap = new Command(() =>  ExecutePushPageCommand<LanguageView>())
                                    },
                                    new SwitchMenuItem {
                                        Icon = ContestParkIcon.SoundSettings,
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
                                        Icon = ContestParkIcon.AccountSettings,
                                        Title = ContestParkResources.EditProfile,
                                        MenuType = MenuTypes.Label,
                                        SingleTap = new Command(() =>  ExecutePushPageCommand<AccountSettingsView>())
                                    },
                                    new TextMenuItem {
                                        Icon = ContestParkIcon.BlockedSettings,
                                        Title = ContestParkResources.Blocking,
                                        MenuType = MenuTypes.Label,
                                        SingleTap = new Command(() =>  ExecutePushPageCommand<BlockingView>())
                                    },
                                    new SwitchMenuItem {
                                        Icon = ContestParkIcon.PrivateProfileSettings,
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
                                        Icon = ContestParkIcon.Exit,
                                        Title = ContestParkResources.LogOut,
                                        MenuType = MenuTypes.Label,
                                        SingleTap = new Command(async()=> await ExitAppAsync()),
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },
            };

            IsBusy = false;

            return base.InitializeAsync(parameters);
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

            _settingsService.AddOrUpdateValue(!_settingsService.IsSoundEffectActive, nameof(_settingsService.IsSoundEffectActive));
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

        #endregion Methods
    }
}
