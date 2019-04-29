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
using Prism.Services;
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
                               IPageDialogService pageDialogService,
                               IIdentityService identityService,
                               ISettingsService settingsService) : base(navigationService, pageDialogService)
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
        /// Üye olma validasyonu
        /// </summary>
        /// <returns>Validasyona takıldıysa true</returns>
        private async Task<bool> CheckValidationAsync()
        {
            string validationMessage = string.Empty;
            if (string.IsNullOrEmpty(SignUpModel.UserName)
                     || string.IsNullOrEmpty(SignUpModel.Fullname)
                     || string.IsNullOrEmpty(SignUpModel.Email)
                     || string.IsNullOrEmpty(SignUpModel.Password)) validationMessage = ContestParkResources.RequiredFields;// Tüm alanlar boş ise
            else if (string.IsNullOrEmpty(SignUpModel.UserName)) validationMessage = ContestParkResources.UserNameRequiredFields;// Kullanıcı adı boş ise
            else if (string.IsNullOrEmpty(SignUpModel.Fullname)) validationMessage = ContestParkResources.FullNameRequiredFields;// Ad soyad boş ise
            else if (string.IsNullOrEmpty(SignUpModel.Email)) validationMessage = ContestParkResources.EmailRequiredFields;// Eposta boş ise
            else if (string.IsNullOrEmpty(SignUpModel.Password)) validationMessage = ContestParkResources.PasswordRequiredFields;// Şifre adı boş ise
            else if (SignUpModel.UserName.Length < 3) validationMessage = ContestParkResources.UserNameMinLength;// Kullanocı adı 3 karakterden küçük olamaz
            else if (SignUpModel.UserName.Length > 255) validationMessage = ContestParkResources.UserNameMaxLength;// Kullanocı adı 255 karakterden büyük olamaz
            else if (SignUpModel.Fullname.Length < 3) validationMessage = ContestParkResources.FullNameMinLength;// Ad soyad 3 karakterden küçük olamaz
            else if (SignUpModel.Fullname.Length > 255) validationMessage = ContestParkResources.FullNameMaxLength;// Ad soyad 255 karakterden büyük olamaz
            else if (SignUpModel.Email.Length > 255) validationMessage = ContestParkResources.EmailMaxLength;// Eposta adresi 255 karakterden büyük olamaz
            // TODO: Eposta adresi doğru formatta mı kontrol edilcek
            else if (SignUpModel.Password.Length < 8) validationMessage = ContestParkResources.PasswordMinLength;// Kullanocı adı 8 karakterden küçük olamaz
            else if (SignUpModel.Password.Length > 32) validationMessage = ContestParkResources.PasswordMaxLength; // Kullanocı adı 32 karakterden büyük olamaz

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        validationMessage,
                                        ContestParkResources.Okay);

                UserDialogs.Instance.HideLoading();

                IsBusy = false;
            }

            return !string.IsNullOrWhiteSpace(validationMessage);
        }

        /// <summary>
        /// Üye ol fonksiyonu
        /// </summary>
        private async Task ExecuteSignUpCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            if (await CheckValidationAsync())
                return;

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
                _settingsService.SetTokenInfo(token);

                await PushNavigationPageAsync($"app:///{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}?appModuleRefresh=OnInitialized");
            }
            else
            {
                await DisplayAlertAsync("",
                    ContestParkResources.MembershipWasSuccessfulButTheLoginFailedPleaseLoginFromTheLoginPage,
                    ContestParkResources.Okay);
            }
        }

        #endregion Methods

        #region Commands

        public ICommand SignUpCommand => new Command(async () => await ExecuteSignUpCommand());

        #endregion Commands
    }
}