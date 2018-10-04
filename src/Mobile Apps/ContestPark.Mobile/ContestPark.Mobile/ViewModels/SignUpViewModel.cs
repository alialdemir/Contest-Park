using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Login;
using ContestPark.Mobile.Models.Token;
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
    public class SignUpViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public SignUpViewModel(INavigationService navigationService,
                               IIdentityService identityService,
                               ISettingsService settingsService) : base(navigationService)
        {
            _identityService = identityService;
            _settingsService = settingsService;
            Title = ContestParkResources.SignUp;
        }

        #endregion Constructor

        #region Properties

        private SignUpModel _signUpModel = new SignUpModel();

        public SignUpModel SignUpModel
        {
            get
            {
                return _signUpModel;
            }
            set
            {
                _signUpModel = value;
                RaisePropertyChanged(() => SignUpModel);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Üye ol fonksiyonu
        /// </summary>
        private async Task ExecuteSignUpCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            var r = SignUpModel;
            var d = SignUpModel.Email;
            var rd = SignUpModel.Fullname;
            var rd1 = SignUpModel.Password;
            var rd2 = SignUpModel.UserName;

            bool isRegister = await _identityService.SignUpAsync(SignUpModel);
            if (isRegister)
                await LoginProcessAsync();

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
                UserName = SignUpModel.UserName,
                Password = SignUpModel.Password
            });
            if (token != null)
            {
                _settingsService.AuthAccessToken = token.AccessToken;
                _settingsService.Language = Enums.Languages.Turkish;// TODO: servisden al
                await PushNavigationPageAsync($"app:///{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}?appModuleRefresh=OnInitialized");
            }
        }

        #endregion Methods

        #region Commands

        public ICommand SignUpCommand => new Command(async () => await ExecuteSignUpCommand());

        #endregion Commands
    }
}