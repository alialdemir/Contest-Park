using Acr.UserDialogs;
using ContestPark.Mobile.Converters.SignInSocialNetworkPage;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static ContestPark.Mobile.Converters.SignInSocialNetworkPage.SignInSocialNetworkPage;

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

        public string FacebookIcon
        {
            get { return "fab-facebook-square"; }
        }

        #endregion Properties

        #region Methods

        private async Task ExecuteSignUpCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await PushNavigationPageAsync($"{nameof(SignUpView)}");
            IsBusy = false;
        }

        private async Task ExecuteForgetYourPasswordCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await PushNavigationPageAsync($"{nameof(ForgetYourPasswordView)}");
            IsBusy = false;
        }

        private async Task ExecuteSignInCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            UserToken token = await _identityService.GetTokenAsync(LoginModel);
            if (token != null)
            {
                _settingsService.AuthAccessToken = token.AccessToken;
                _settingsService.Language = Enums.Languages.Turkish;// TODO: servisden al
                await PushNavigationPageAsync($"app:///{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}?appModuleRefresh=OnInitialized");
            }

            UserDialogs.Instance.HideLoading();

            IsBusy = false;
        }

        /// <summary>
        /// <param name="socialNetworkType"></param> göre sosyal medyada login işlemi gerçekleştirir
        /// </summary>
        /// <param name="socialNetworkType">Hangi sosyal medya ile login olduğu</param>
        private void ExecuteSocialNetworkkWithLoginCommand(string socialNetworkType)
        {
            if (string.IsNullOrEmpty(socialNetworkType))
                return;

            SocialNetworkTypes socialNetworkTypeEnum = (SocialNetworkTypes)byte.Parse(socialNetworkType);

            if (!SocialNetworkTypes.Facebook.HasFlag(socialNetworkTypeEnum) ||
                !SocialNetworkTypes.Twitter.HasFlag(socialNetworkTypeEnum) ||
                !SocialNetworkTypes.GooglePlus.HasFlag(socialNetworkTypeEnum))
                return;

            ContestParkApp.Current.MainPage.Navigation.PushModalAsync(new SignInSocialNetworkPage(socialNetworkTypeEnum)
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
        public ICommand ForgetYourPasswordCommand => new Command(async () => await ExecuteForgetYourPasswordCommand());

        private ICommand socialNetworkkWithLoginCommand;

        public ICommand SocialNetworkkWithLoginCommand
        {
            get
            {
                return socialNetworkkWithLoginCommand ?? (socialNetworkkWithLoginCommand = new Command<string>((socialNetworkType) =>
                                                                                                                            ExecuteSocialNetworkkWithLoginCommand(socialNetworkType)));
            }
        }

        #endregion Commands
    }
}