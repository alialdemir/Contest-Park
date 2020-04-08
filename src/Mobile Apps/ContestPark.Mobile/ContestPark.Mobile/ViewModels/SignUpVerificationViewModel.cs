using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Models.RequestProvider;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
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

        public SignUpVerificationViewModel(IIdentityService identityService,
                                           INavigationService navigationService,
                                           INotificationService notificationService,
                                           IPageDialogService dialogService,
                                           ISettingsService settingsService) : base(navigationService: navigationService,
                                                                                    dialogService: dialogService)
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
            if (SmsInfo.PhoneNumber.StartsWith("5454"))
            {
                Code1 = 5;
                Code2 = 4;
                Code3 = 5;
                Code4 = 4;
            }

            SendSmsCommand.Execute(null);

            await base.InitializeAsync();
        }

        /// <summary>
        /// SMS ile gelen kodu kontrol eder ve telefon numarası kayıtlı mı diye kontrol eder eğer kayıtlı ise login yapar
        /// eğer kayıtlı değilse kayıt olma popupu başlangıcı olan
        /// </summary>
        private async void ExecuteCheckSmsCodeCommand()
        {
            if (!Code1.HasValue || !Code2.HasValue || !Code3.HasValue || !Code4.HasValue)
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.IncorrectSMSCode,
                                        ContestParkResources.Okay);
                return;
            }

            ResponseModel<UserNameModel> response = await _notificationService.CheckSmsCode(new SmsModel
            {
                Code = Convert.ToInt32($"{Code1}{Code2}{Code3}{Code4}"),
                PhoneNumber = SmsInfo.PhoneNumber
            });

            if (!response.IsSuccess)// Sms kodunu yanlış girmiştir
            {
                await DisplayAlertAsync(string.Empty,
                                        ContestParkResources.IncorrectSMSCode,
                                        ContestParkResources.Okay);
                return;
            }

            if (response.IsSuccess && !string.IsNullOrEmpty(response.Data.UserName))// Sms kodu doğru ve kullanıcı kayıtlı
            {
                bool isLogin = await Login(response.Data.UserName);
                if (isLogin)
                    return;
            }

            /*
             *  response içindeki kullanıcı adı boş ise sms doğrulama yapılmıştır
             *  ancak telefon numarasına ait kullanıcı yoktur o yüzden üye olma adımlarına yönlendirdik
             */

            GotoBackCommand.Execute(true);

            await PushModalAsync(nameof(SignUpFullNameView), new NavigationParameters
            {
                { "PhoneNumber", SmsInfo.PhoneNumber }
            });
        }

        /// <summary>
        /// Login işlemi
        /// </summary>
        /// <returns>Login başarılı ise true değilse false</returns>
        private async Task<bool> Login(string userName)
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            if (!string.IsNullOrEmpty(userName))
            {
                await SignInAsync(userName);

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
            if (SmsInfo.PhoneNumber.StartsWith("5454"))// Eğer numaranın başı 5454 ile başlıyorsa sms göndermeden login olmalı özel durumlar için ekledim
            {
                SmsInfo.PhoneNumber = SmsInfo.PhoneNumber.Substring(4, SmsInfo.PhoneNumber.Length - 4);
            }

            UserToken token = await _identityService.GetTokenAsync(new Models.LoginModel
            {
                Password = SmsInfo.PhoneNumber,
                UserName = userName
            });
            if (token != null)
            {
                _settingsService.SetTokenInfo(token);

                UserInfoModel currentUser = await _identityService.GetUserInfo();
                if (currentUser != null)
                {
                    _settingsService.RefreshCurrentUser(currentUser);
                }

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

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            return base.GoBackAsync(parameters, useModalNavigation: true);
        }

        /// <summary>
        /// Tekrar sms gönder
        /// </summary>
        private async void ExecuteSendSmsCommand()
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

            await _notificationService.LogInSms(SmsInfo);
        }

        #endregion Methods

        #region Commands

        private ICommand TimeLeftCommand => new Command(ExecuteTimeLeftCommand);
        public ICommand CheckSmsCodeCommand => new Command(ExecuteCheckSmsCodeCommand);

        public ICommand SendSmsCommand => new Command(ExecuteSendSmsCommand);

        #endregion Commands

        #region Navgation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SmsInfo"))
                SmsInfo = parameters.GetValue<SmsInfoModel>("SmsInfo");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navgation
    }
}
