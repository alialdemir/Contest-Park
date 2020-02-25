using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.Identity;
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
        #region Constructor

        public PhoneNumberViewModel(IPopupNavigation popupNavigation,
                                    INavigationService navigationService,
                                    ISettingsService settingsService,
                                    IIdentityService identityService,
                                    IPageDialogService dialogService) : base(navigationService: navigationService,
                                                                             dialogService: dialogService,
                                                                             popupNavigation: popupNavigation)
        {
            this._settingsService = settingsService;
            this._identityService = identityService;
#if DEBUG
            PhoneNumber = "5444261154";
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

        private readonly ISettingsService _settingsService;
        private readonly IIdentityService _identityService;

        public CountryModel Country
        {
            get { return _country; }
            set
            {
                _country = value;
                RaisePropertyChanged(() => Country);
            }
        }

        public string PhoneNumberNoRegex { get; set; }

        #endregion Properties

        #region Methods

        protected override Task InitializeAsync()
        {
            if (!_settingsService.IsTutorialDisplayed && Device.RuntimePlatform == Device.Android)
            {
                PushPopupPageAsync(new TutorialPopupView());

                _settingsService.IsTutorialDisplayed = true;
            }

            return base.InitializeAsync();
        }

        private void OnCountry(object sender, CountryModel e)
        {
            if (e != null)
            {
                Country = e;
            }
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

            PhoneNumberNoRegex = PhoneNumber
            .Replace("(", "")
            .Replace(")", "")
            .Replace(" ", "")
            .Replace("-", "");

            var match = Regex.Match(PhoneNumberNoRegex, @"^5(0[5-7]|[3-5]\d) ?\d{3} ?\d{4}$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.InvalidPhoneNumber,
                                        ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            // TODO: send sms
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            // TODO: check sms

            string userName = await _identityService.GetUserNameByPhoneNumber(PhoneNumberNoRegex);
            if (!string.IsNullOrEmpty(userName))
            {
                await SignInAsync(userName);

                UserDialogs.Instance.HideLoading();

                IsBusy = false;

                return;
            }

            UserDialogs.Instance.HideLoading();

            //    await RemoveFirstPopupAsync();

            await PushPopupPageAsync(new SignUpFullNameView()
            {
                PhoneNumber = PhoneNumberNoRegex
            });
            //int smsCode = new Random().Next(100000, 999999);

            //await PushPopupPageAsync(new CheckSmsView
            //{
            //    PhoneNumber = phoneNumber,
            //    SmsCode = smsCode,
            //});

            IsBusy = false;
        }

        /// <summary>
        /// Ülke seçme popup açar
        /// </summary>
        private async Task ExecuteSelectCountryCommandAsync()
        {
            var selectCountryView = new SelectCountryView();
            selectCountryView.CountryEventHandler += OnCountry;

            await PushPopupPageAsync(selectCountryView);
        }

        /// <summary>
        /// Giriş yap
        /// </summary>
        private async Task SignInAsync(string userName)// sms login gelince sil
        {
            UserToken token = await _identityService.GetTokenAsync(new Models.LoginModel
            {
                Password = PhoneNumberNoRegex,
                UserName = userName
            });
            if (token != null)
            {
                _settingsService.SetTokenInfo(token);

                await PushNavigationPageAsync($"app:///{nameof(AppShell)}?appModuleRefresh=OnInitialized");
            }

            UserDialogs.Instance.HideLoading();
        }

        #endregion Methods

        #region Commands

        public ICommand PrivacyPolicyCommand
        {
            get
            {
                return new Command(() => Launcher.OpenAsync(new Uri("http://contestpark.com/privacy-policy.html")));
            }
        }

        public ICommand SendSmsCommand => new Command(async () => await ExecuteSendSmsCommandAsync());
        public ICommand SelectCountryCommand => new Command(async () => await ExecuteSelectCountryCommandAsync());

        #endregion Commands
    }
}
