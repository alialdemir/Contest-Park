using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PhoneNumberViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public PhoneNumberViewModel(IPopupNavigation popupNavigation,
                                    INavigationService navigationService,
                                    ISettingsService settingsService,
                                    IPageDialogService dialogService) : base(navigationService: navigationService,
                                                                             dialogService: dialogService,
                                                                             popupNavigation: popupNavigation)
        {
            this._settingsService = settingsService;
#if DEBUG
            PhoneNumber = "54545444261154";
#endif
        }

        #endregion Constructor

        #region Properties

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;

                RaisePropertyChanged(() => PhoneNumber);
            }
        }

        private CountryModel _country = new CountryModel()
        {
            Country = "Türkiye",
            Flag = Device.RuntimePlatform == Device.Android ? "assets/images/TUR_s.png" : "TUR_s.png",
            PhoneCode = "+90"
        };

        public CountryModel Country
        {
            get { return _country; }
            set
            {
                _country = value;
                RaisePropertyChanged(() => Country);
            }
        }

        private string PhoneNumberNoRegex
        {
            get
            {
                return PhoneNumber
                            .Replace("(", "")
                            .Replace(")", "")
                            .Replace(" ", "")
                            .Replace("-", "");
            }
        }

        #endregion Properties

        #region Methods

        protected override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("SelectedCountry"))
            {
                var selectedCountry = parameters.GetValue<CountryModel>("SelectedCountry");
                if (selectedCountry != null)
                {
                    Country = selectedCountry;
                }
            }

            if (!_settingsService.IsTutorialDisplayed && Device.RuntimePlatform == Device.Android)
            {
                NavigateToPopupAsync<TutorialPopupView>();

                _settingsService.IsTutorialDisplayed = true;
            }

            return base.InitializeAsync(parameters);
        }

        /// <summary>
        /// Random sms kodu oluşturup sms gönderir ve CheckSmsView'e yönlendirir
        /// </summary>
        private async Task ExecuteSendSmsCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (string.IsNullOrEmpty(PhoneNumber))// Kullanıcı adı boş ise
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.PhoneNumberRequiredFields,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }

            var match = Regex.Match(PhoneNumberNoRegex, @"^5(0[5-7]|[3-5]\d) ?\d{3} ?\d{4}$", RegexOptions.IgnoreCase);
            if (!match.Success && !PhoneNumberNoRegex.StartsWith("5454"))// özel durumlar için 1993 ekledim
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.InvalidPhoneNumber,
                                        ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            await NavigateToPopupAsync<SignUpVerificationView>(new NavigationParameters
            {
                {
                      "SmsInfo", new SmsInfoModel
                                            {
                                                PhoneNumber = PhoneNumberNoRegex,
                                                CountryCode = Country.PhoneCode
                                            }
                }
            });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand OpenLinkCommand
        {
            get
            {
                return new Command<string>((url) => Launcher.OpenAsync(new Uri(url)));
            }
        }

        public ICommand SendSmsCommand => new CommandAsync(ExecuteSendSmsCommandAsync);

        /// <summary>
        /// Ülke seçme popup açar
        /// </summary>
        public ICommand SelectCountryCommand => new Command(() => NavigateToPopupAsync<SelectCountryView>());

        #endregion Commands
    }
}
