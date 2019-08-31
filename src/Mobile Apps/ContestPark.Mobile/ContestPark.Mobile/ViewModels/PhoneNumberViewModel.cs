using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Country;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PhoneNumberViewModel : ViewModelBase
    {
        #region Constructor

        public PhoneNumberViewModel(IPopupNavigation popupNavigation,
                                    IPageDialogService dialogService) : base(dialogService: dialogService, popupNavigation: popupNavigation)
        {
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
            Flag = "assets/images/TUR_s.png",
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

        #endregion Properties

        #region Methods

        private async void OnCountry(object sender, CountryModel e)
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

            string phoneNumber = PhoneNumber
            .Replace("(", "")
            .Replace(")", "")
            .Replace(" ", "")
            .Replace("-", "");

            var match = Regex.Match(phoneNumber, @"^5(0[5-7]|[3-5]\d) ?\d{3} ?\d{4}$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.InvalidPhoneNumber,
                                        ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            // TODO: send sms

            int smsCode = new Random().Next(100000, 999999);

            await PushPopupPageAsync(new CheckSmsView
            {
                PhoneNumber = phoneNumber,
                SmsCode = smsCode,
            });

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

        #endregion Methods

        #region Commands

        public ICommand SendSmsCommand => new Command(async () => await ExecuteSendSmsCommandAsync());
        public ICommand SelectCountryCommand => new Command(async () => await ExecuteSelectCountryCommandAsync());

        #endregion Commands
    }
}
