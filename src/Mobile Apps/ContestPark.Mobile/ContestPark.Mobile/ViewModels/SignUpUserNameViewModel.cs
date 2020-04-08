using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpUserNameViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public SignUpUserNameViewModel(INavigationService navigationService,
                                       ISettingsService settingsService,
                                       IAnalyticsService analyticsService,
                                       IPageDialogService dialogService,
                                       IIdentityService identityService) : base(navigationService: navigationService,
                                                                                dialogService: dialogService)
        {
            _settingsService = settingsService;
            _analyticsService = analyticsService;
            _identityService = identityService;
        }

        #endregion Constructor

        #region Properties

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string ReferenceCode { get; set; }

        #endregion Properties

        #region Methods

        private async Task ExecuteUserNameCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (string.IsNullOrEmpty(UserName))// Kullanıcı adı boş ise
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.UserNameRequiredFields,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }
            else if (UserName.Length < 3)// Kullanıcı adı 3 karakterden küçük olamaz
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.UserNameMinLength,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }
            else if (UserName.Length > 255)// Kullanıcı adı 255 karakterden büyük olamaz
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.UserNameMaxLength,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            bool isSuccess = await _identityService.SignUpAsync(new SignUpModel
            {
                FullName = FullName,
                Password = PhoneNumber,
                UserName = UserName,
                ReferenceCode = ReferenceCode
            });

            if (isSuccess)
            {
                await LoginProcessAsync();
            }
            UserDialogs.Instance.HideLoading();

            IsBusy = false;
        }

        /// <summary>
        /// Üye olduktan sonra login olmakk için
        /// </summary>
        private async Task LoginProcessAsync()
        {
            UserToken token = await _identityService.GetTokenAsync(new LoginModel
            {
                UserName = UserName,
                Password = PhoneNumber
            });

            bool isTokenExits = token != null;

            _analyticsService.SendEvent("Login", "Üye ol", isTokenExits ? "Success" : "Fail");

            if (isTokenExits)
            {
                _settingsService.SetTokenInfo(token);

                UserInfoModel currentUser = await _identityService.GetUserInfo();
                if (currentUser != null)
                {
                    _settingsService.RefreshCurrentUser(currentUser);
                }

                _settingsService.SignUpCount += 1;

                _analyticsService.SetUserId(_settingsService.CurrentUser.UserId);

                await PushNavigationPageAsync($"app:///{nameof(AppShell)}?appModuleRefresh=OnInitialized");
            }
            else
            {
                await DisplayAlertAsync("",
                    ContestParkResources.MembershipWasSuccessfulButTheLoginFailedPleaseLoginFromTheLoginPage,
                    ContestParkResources.Okay);
            }
        }

        /// <summary>
        /// Referans kodu girme sayfasına yönlendirir
        /// </summary>
        private void ExecuteGotoReferenceCodeCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PushModalAsync(nameof(SignUpReferenceCodeView));

            IsBusy = false;
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            return base.GoBackAsync(parameters, useModalNavigation: true);
        }

        #endregion Methods

        #region Commands

        public ICommand UserNameCommand => new Command(async () => await ExecuteUserNameCommandAsync());
        public ICommand GotoReferenceCodeCommand => new Command(ExecuteGotoReferenceCodeCommand);

        #endregion Commands

        #region Navgation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("FullName"))
                FullName = parameters.GetValue<string>("FullName");

            if (parameters.ContainsKey("PhoneNumber"))
                PhoneNumber = parameters.GetValue<string>("PhoneNumber");

            if (parameters.ContainsKey("ReferenceCode"))
                ReferenceCode = parameters.GetValue<string>("ReferenceCode");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navgation
    }
}
