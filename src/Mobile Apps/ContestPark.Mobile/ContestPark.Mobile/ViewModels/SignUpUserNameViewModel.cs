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
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpUserNameViewModel : ViewModelBase
    {
        #region Private variables

        private readonly ISettingsService _settingsService;

        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public SignUpUserNameViewModel(IPopupNavigation popupNavigation,
                                       INavigationService navigationService,
                                       ISettingsService settingsService,
                                       IPageDialogService dialogService,
                                       IIdentityService identityService) : base(navigationService: navigationService,
                                                                                dialogService: dialogService,
                                                                                popupNavigation: popupNavigation)
        {
            _settingsService = settingsService;
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

            bool isRegister = await _identityService.SignUpAsync(new SignUpModel
            {
                FullName = FullName,
                Password = PhoneNumber,
                UserName = UserName,
            });

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
                UserName = UserName,
                Password = PhoneNumber
            });

            if (token != null)
            {
                _settingsService.SetTokenInfo(token);

                _settingsService.SignUpCount += 1;

                await PushNavigationPageAsync($"app:///{nameof(AppShell)}?appModuleRefresh=OnInitialized");
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

        public ICommand UserNameCommand => new Command(async () => await ExecuteUserNameCommandAsync());

        #endregion Commands
    }
}
