using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PhoneNumberViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IEventAggregator _eventAggregator;

        private readonly ISettingsService _settingsService;
        private SubscriptionToken _subscriptionToken;

        #endregion Private variables

        #region Constructor

        public PhoneNumberViewModel(INavigationService navigationService,
                                    IPageDialogService dialogService,
                                    IEventAggregator eventAggregator,
                                    ISettingsService settingsService) : base(navigationService, dialogService)
        {
            _eventAggregator = eventAggregator;
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

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            Microsoft.AppCenter.Crashes.Crashes.TrackError(new System.Exception("phone number"));
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
                Device.BeginInvokeOnMainThread(() =>
                 {
                     NavigateToPopupAsync<TutorialPopupView>();

                     _settingsService.IsTutorialDisplayed = true;
                 });
            }

            _subscriptionToken = _eventAggregator
                                         .GetEvent<NavigateToInitializedEvent>()
                                         .Subscribe(async () =>
                                         {
                                             await GoBackAsync();
                                             await NavigateToInitialized<AppShell>();
                                         });

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

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            _eventAggregator
                .GetEvent<NavigateToInitializedEvent>()
                .Unsubscribe(_subscriptionToken);

            return base.GoBackAsync(parameters, useModalNavigation);
        }

        #endregion Methods

        #region Commands

        public ICommand OpenLinkCommand
        {
            get
            {
                return new Command<string>((url) =>
                {
                    NavigateToAsync<BrowserView>(new NavigationParameters
                                                        {
                                                            { "Link", url }
                                                        });
                });
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
