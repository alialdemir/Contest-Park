using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class LanguageViewModel : ViewModelBase<MenuItemList>
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public LanguageViewModel(INavigationService navigationService,
                                 ISettingsService settingsService,
                                 IIdentityService identityService) : base(navigationService)
        {
            Title = ContestParkResources.Language;

            _settingsService = settingsService;
            _identityService = identityService;
        }

        #endregion Constructors

        #region Methods

        protected override Task InitializeAsync()
        {
            bool isTurkish = _settingsService.CurrentUser.Language == Languages.Turkish;

            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.SelectLanguage)
                                {
                                    new SwitchMenuItem {
                                        CommandParameter = Languages.Turkish,
                                        Title = ContestParkResources.Turkish,
                                        MenuType = MenuTypes.Switch,
                                        SingleTap = ChangeLanguageCommand,
                                        IsToggled = isTurkish
                                    },
                                    new SwitchMenuItem {
                                        CommandParameter = Languages.English,
                                        Title = ContestParkResources.English,
                                        MenuType = MenuTypes.Switch,
                                        SingleTap = ChangeLanguageCommand,
                                        IsToggled = !isTurkish,
                                        CornerRadius = new CornerRadius(0,0,8,8)
                                    },
                                },
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Device üzerinden kültür değerlerini set eder
        /// </summary>
        /// <param name="languageCode">Language code</param>
        private void ChangeDeviceCulture(string languageCode)
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                var culture = new CultureInfo(languageCode);

                ContestParkResources.Culture = culture;

                DependencyService.Get<ILocalize>().SetCultureInfo(culture);
            }
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="langName"></param>
        private async Task ExecuteChangeLanguageCommand(Languages language)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            SwipedTogleChanges(language);

            string languageCode = language.ToLanguageCode();

            await _settingsService.SetSettingsAsync(SettingTypes.Language, languageCode);

            _settingsService.CurrentUser.Language = language;

            ChangeDeviceCulture(languageCode);

            await _identityService.RefreshTokenAsync();

            await PushNavigationPageAsync($"app:///{nameof(AppShell)}?appModuleRefresh=OnInitialized");

            UserDialogs.Instance.HideLoading();

            IsBusy = false;
        }

        /// <summary>
        /// Seçilen dil dışındaki dillerin IsToggled falseyapar
        /// </summary>
        /// <param name="language">Seçilen dil adı</param>
        private void SwipedTogleChanges(Languages language)
        {
            Items
                 .FirstOrDefault()
                 .Where(p => (Languages)((SwitchMenuItem)p).CommandParameter != language)
                 .ToList()
                 .ForEach(p => ((SwitchMenuItem)p).IsToggled = false);
        }

        #endregion Methods

        #region Commands

        private ICommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand
        {
            get { return _changeLanguageCommand ?? (_changeLanguageCommand = new Command<Languages>(async (language) => await ExecuteChangeLanguageCommand(language))); }
        }

        #endregion Commands
    }
}
