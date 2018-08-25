using Acr.UserDialogs;
using ContestPark.Mobile.Converters.SignInSocialNetworkPage;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public SignInViewModel(INavigationService navigationService,
                               IIdentityService identityService,
                               ISettingsService settingsService) : base(navigationService)
        {
            _identityService = identityService;
            _settingsService = settingsService;

#if (DEBUG)
            LoginModel.UserName = "witcherfearless";
            LoginModel.Password = "19931993";
#endif
        }

        #endregion Constructor

        #region Properties

        private LoginModel _loginModel = new LoginModel();

        public LoginModel LoginModel
        {
            get
            {
                return _loginModel;
            }
            set
            {
                _loginModel = value;
                RaisePropertyChanged(() => LoginModel);
            }
        }

        private bool _isValid;

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        #endregion Properties

        #region Methods

        private async Task ExecuteSignUpCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await PushModalAsync($"{nameof(BaseNavigationPage)}/{nameof(SignUpView)}");
            IsBusy = false;
        }

        private async Task ExecuteForgetYourPasswordCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await PushModalAsync($"{nameof(BaseNavigationPage)}/{nameof(ForgetYourPasswordView)}");
            IsBusy = false;
        }

        private async Task ExecuteSignInCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            UserDialogs.Instance.ShowLoading("", MaskType.Black);
            var token = await _identityService.GetTokenAsync(LoginModel);

            if (token != null)
            {
                _settingsService.AuthAccessToken = token.AccessToken;
                // bu hataya neden oluyor kapattık   _settingsService.AuthIdToken = token.IdToken;
                _settingsService.Language = Enums.Languages.Turkish;// servisden al
                await PushNavigationPageAsync($"app:///{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}?appModuleRefresh=OnInitialized");
            }

            UserDialogs.Instance.HideLoading();
            IsValid = true;
            IsBusy = false;
        }

        private void ExecuteFacebookWithLoginCommand()
        {
            ContestParkApp.Current.MainPage.Navigation.PushModalAsync(new SignInSocialNetworkPage(SignInSocialNetworkPage.SocialNetworkTypes.Facebook)
            {
                CompletedCommand = new Command<string>((accessToken) =>
                {
                }),
                ErrorCommand = new Command<string>((errror) =>
                {
                })
            });
        }

        #endregion Methods

        #region Commands

        public ICommand SignUpCommand => new Command(async () => await ExecuteSignUpCommand());
        public ICommand SignInCommand => new Command(async () => await ExecuteSignInCommandAsync());
        public ICommand FacebookWithLoginCommand => new Command(() => ExecuteFacebookWithLoginCommand());
        public ICommand ForgetYourPasswordCommand => new Command(async () => await ExecuteForgetYourPasswordCommand());

        #endregion Commands
    }
}