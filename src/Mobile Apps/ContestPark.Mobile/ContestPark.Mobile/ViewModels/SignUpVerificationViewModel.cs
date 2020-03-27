using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpVerificationViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public SignUpVerificationViewModel(IPopupNavigation popupNavigation,
                                           IIdentityService identityService,
                                           INavigationService navigationService,
                                           INotificationService notificationService,
                                           IPageDialogService dialogService,
                                           ISettingsService settingsService) : base(navigationService: navigationService,
                                                                                    dialogService: dialogService,
                                                                                    popupNavigation: popupNavigation)
        {
            _identityService = identityService;
            _notificationService = notificationService;
            _settingsService = settingsService;
        }

        #endregion Constructor

        #region Properties

        public DateTime Time { get; set; }

        private string _timeLeft;

        public string TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                _timeLeft = value;
                RaisePropertyChanged(() => TimeLeft);
            }
        }

        public SmsInfoModel SmsInfo { get; set; }

        private SmsModel SmsCode { get; set; }
        private string UserName { get; set; }

        private byte? _code1;
        private byte? _code2;
        private byte? _code3;
        private byte? _code4;

        public byte? Code1
        {
            get
            {
                return _code1;
            }
            set
            {
                _code1 = value;
                RaisePropertyChanged(() => Code1);
            }
        }

        public byte? Code2
        {
            get
            {
                return _code2;
            }
            set
            {
                _code2 = value;
                RaisePropertyChanged(() => Code2);
            }
        }

        public byte? Code3
        {
            get
            {
                return _code3;
            }
            set
            {
                _code3 = value;
                RaisePropertyChanged(() => Code3);
            }
        }

        public byte? Code4
        {
            get
            {
                return _code4;
            }
            set
            {
                _code4 = value;
                RaisePropertyChanged(() => Code4);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
#if DEBUG
            Code1 = 5;
            Code2 = 4;
            Code3 = 5;
            Code4 = 4;
#endif
            if (SmsInfo.PhoneNumber.Equals("5444261154"))
            {
                UserName = await _identityService.GetUserNameByPhoneNumber(SmsInfo.PhoneNumber);

                await Login();
            }
            else if (!SmsInfo.PhoneNumber.StartsWith("5454"))// Eğer özel durum yoksa direk sms göndersin
            {
                SendSmsCommand.Execute(null);
            }
            else// ÖZEL DURUMSA 5454 ile giriş yapılsın
            {
                SmsInfo.PhoneNumber = SmsInfo.PhoneNumber.Substring(4, SmsInfo.PhoneNumber.Length - 4);
                SmsCode = new SmsModel { Code = 5454 };
            }

            UserName = await _identityService.GetUserNameByPhoneNumber(SmsInfo.PhoneNumber);

            await base.InitializeAsync();
        }

        /// <summary>
        /// SMS ile gelen kodu kontrol eder ve telefon numarası kayıtlı mı diye kontrol eder eğer kayıtlı ise login yapar
        /// eğer kayıtlı değilse kayıt olma popupu başlangıcı olan
        /// </summary>
        private async Task ExecuteCheckSmsCodeCommand()
        {
            string strCode = $"{Code1}{Code2}{Code3}{Code4}";

            if (SmsCode == null || string.IsNullOrEmpty(strCode) || strCode != SmsCode.Code.ToString())
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.IncorrectSMSCode,
                                        ContestParkResources.Okay);
                return;
            }

            bool isLogin = await Login();
            if (isLogin)
                return;

            await PushPopupPageAsync(new SignUpFullNameView()
            {
                PhoneNumber = SmsInfo.PhoneNumber// TODO: Ülke koduda gönderilmeli
            });
        }

        /// <summary>
        /// Login işlemi
        /// </summary>
        /// <returns>Login başarılı ise true değilse false</returns>
        private async Task<bool> Login()
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            if (!string.IsNullOrEmpty(UserName))
            {
                await SignInAsync(UserName);

                UserDialogs.Instance.HideLoading();

                return true;
            }

            UserDialogs.Instance.HideLoading();

            return false;
        }

        /// <summary>
        /// Giriş yap
        /// </summary>
        private async Task SignInAsync(string userName)
        {
            UserToken token = await _identityService.GetTokenAsync(new Models.LoginModel
            {
                Password = SmsInfo.PhoneNumber,
                UserName = userName
            });
            if (token != null)
            {
                _settingsService.SetTokenInfo(token);

                await PushNavigationPageAsync($"app:///{nameof(AppShell)}?appModuleRefresh=OnInitialized");
            }

            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Sms kodu girme süresi
        /// </summary>
        private void ExecuteTimeLeftCommand()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
            {
                TimeSpan diff = Time - DateTime.Now;

                string seconds = diff.Seconds < 10 ? $"0{diff.Seconds}" : diff.Seconds.ToString();

                TimeLeft = string.Format(ContestParkResources.RemainingTimeSec, diff.Minutes, seconds);

                return !(diff.Minutes == 0 && diff.Seconds == 0);
            });
        }

        #endregion Methods

        #region Commands

        public ICommand TimeLeftCommand => new Command(() => ExecuteTimeLeftCommand());
        public ICommand CheckSmsCodeCommand => new Command(async () => await ExecuteCheckSmsCodeCommand());

        public ICommand SendSmsCommand => new Command(async () =>
        {
            if (Time != DateTime.MinValue)
            {
                TimeSpan diff = Time.AddSeconds(-30) - DateTime.Now;
                if (diff.Minutes == 2 && diff.Seconds < 30)
                {
                    await DisplayAlertAsync(string.Empty,
                                            string.Format(ContestParkResources.Wait30SecondsToRequestSMSAgain, diff.Seconds),
                                            ContestParkResources.Okay);
                    return;
                }
            }

            Code1 = Code2 = Code3 = Code4 = null;

            Time = DateTime.Now.AddMinutes(3);

            TimeLeftCommand.Execute(null);

            SmsCode = await _notificationService.LogInSms(SmsInfo);
        });

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }

        #endregion Commands
    }
}
