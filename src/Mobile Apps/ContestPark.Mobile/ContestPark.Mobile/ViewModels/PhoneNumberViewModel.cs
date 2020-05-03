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
            Flag = "TUR_s.png".ToResourceImage(),
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

        public override void Initialize(INavigationParameters parameters = null)
        {
            ShowTutorialCommand.Execute(null);

            EventListenerCommand.Execute(null);

            base.Initialize(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedCountry"))
            {
                var selectedCountry = parameters.GetValue<CountryModel>("SelectedCountry");
                if (selectedCountry != null)
                {
                    Country = selectedCountry;
                }
            }

            base.OnNavigatedTo(parameters);
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

        /// <summary>
        /// NavigateToInitializedEvent event listenre
        /// </summary>
        private void ExecuteEventListenerCommand()
        {
            _subscriptionToken = _eventAggregator
                                         .GetEvent<NavigateToInitializedEvent>()
                                         .Subscribe(async () =>
                                         {
                                             await GoBackAsync();
                                             await NavigateToInitialized<AppShell>();
                                         });
        }

        /// <summary>
        /// Daha önce gösterilmediyse tutorial ekranını gösterir
        /// </summary>
        private void ExecuteShowTutorialCommand()
        {
            if (!_settingsService.IsTutorialDisplayed && Device.RuntimePlatform == Device.Android)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NavigateToPopupAsync<TutorialPopupView>();

                    _settingsService.IsTutorialDisplayed = true;
                });
            }
        }

        #endregion Methods

        #region Commands

        private ICommand ShowTutorialCommand => new Command(ExecuteShowTutorialCommand);

        private ICommand EventListenerCommand => new Command(ExecuteEventListenerCommand);

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
