using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Models.MenuItem;
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

        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public LanguageViewModel(INavigationService navigationService,
                                 ISettingsService settingsService) : base(navigationService)
        {
            Title = ContestParkResources.Language;
            _settingsService = settingsService;
        }

        #endregion Constructors

        #region Properties

        public bool IsExit { get; set; }

        #endregion Properties

        #region Methods

        protected override Task InitializeAsync()
        {
            bool isTurkish = _settingsService.Language == Languages.Turkish;

            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList(ContestParkResources.SelectLanguage)
                                {
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = "Turkish",
                                        Title = ContestParkResources.Turkish,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = isTurkish
                                    },
                                    new Models.MenuItem.MenuItem {
                                        CommandParameter = "English",
                                        Title = ContestParkResources.English,
                                        MenuType = Enums.MenuTypes.Switch,
                                        IsToggled = !isTurkish
                                    },
                                },
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="langName"></param>
        private async Task ExecuteChangeLanguageCommand(string langName)
        {
            if (IsExit || IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            Languages language = langName == "Turkish" ? Languages.Turkish : Languages.English;

            SwipedTogleChanges(langName);

            string languageCode = language.ToLanguageCode();

            await _settingsService.SetSettingsAsync(SettingTypes.Language, languageCode);

            _settingsService.Language = language;

            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                var culture = new CultureInfo(languageCode);

                ContestParkResources.Culture = culture;

                DependencyService.Get<ILocalize>().SetCultureInfo(culture);
            }

            await PushNavigationPageAsync($"app:///{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}?appModuleRefresh=OnInitialized");

            IsBusy = false;

            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Seçilen dil dışındaki dillerin IsToggled falseyapar
        /// </summary>
        /// <param name="langName">Seçilen dil adı</param>
        private void SwipedTogleChanges(string langName)
        {
            Items
                 .FirstOrDefault()
                 .Where(p => p.CommandParameter != langName)
                 .ToList()
                 .ForEach(p => p.IsToggled = false);
        }

        #endregion Methods

        #region Commands

        private ICommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand
        {
            get { return _changeLanguageCommand ?? (_changeLanguageCommand = new Command<string>(async (langName) => await ExecuteChangeLanguageCommand(langName))); }
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