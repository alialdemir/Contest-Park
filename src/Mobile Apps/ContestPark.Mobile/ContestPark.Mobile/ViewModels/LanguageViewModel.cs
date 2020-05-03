using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System;
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
        private readonly ICacheService _cacheService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructors

        public LanguageViewModel(INavigationService navigationService,
                                 ICacheService cacheService,
                                 IPageDialogService dialogService,
                                 ISettingsService settingsService,
                                 IIdentityService identityService) : base(navigationService, dialogService: dialogService)
        {
            Title = ContestParkResources.Language;
            _cacheService = cacheService;
            _settingsService = settingsService;
            _identityService = identityService;
        }

        #endregion Constructors

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            SetLanguageCommand.Execute(null);

            base.Initialize(parameters);
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

                Xamarin.Forms.DependencyService.Get<ILocalize>().SetCultureInfo(culture);
            }
        }

        /// <summary>
        /// Parametreden gelen sayfa adına göre işlem yapar
        /// </summary>
        /// <param name="langName"></param>
        private async Task ExecuteChangeLanguageCommand(Languages language)
        {
            if (IsBusy || _settingsService.CurrentUser.Language == language)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            SwipedTogleChanges(language);

            string languageCode = language.ToLanguageCode();

            bool isSuccess = await _identityService.UpdateLanguageAsync(language);
            if (!isSuccess)
            {
                SwipedTogleChanges(_settingsService.CurrentUser.Language);

                UserDialogs.Instance.HideLoading();

                await DisplayAlertAsync("",
                                        ContestParkResources.GlobalErrorMessage,
                                        ContestParkResources.Okay);

                return;
            }

            ChangeDeviceCulture(languageCode);

            _settingsService.CurrentUser.Language = language;

            _settingsService.RefreshCurrentUser(_settingsService.CurrentUser);

            _cacheService.EmptyAll();

            await NavigateToInitialized<AppShell>();

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

        /// <summary>
        /// Dilleri items olarak ekler
        /// </summary>
        private void ExecuteSetLanguageCommand()
        {
            bool isTurkish = _settingsService.CurrentUser.Language == Languages.Turkish;

            ServiceModel = new Models.ServiceModel.ServiceModel<MenuItemList>
            {
                Items = new List<MenuItemList>()
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
                                            },
                Count = 2,
                HasNextPage = false,
                PageSize = 2,
                PageNumber = 1
            };
        }

        #endregion Methods

        #region Commands

        private ICommand SetLanguageCommand => new Command(ExecuteSetLanguageCommand);

        private ICommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand
        {
            get { return _changeLanguageCommand ?? (_changeLanguageCommand = new CommandAsync<Languages>(ExecuteChangeLanguageCommand)); }
        }

        #endregion Commands
    }
}
