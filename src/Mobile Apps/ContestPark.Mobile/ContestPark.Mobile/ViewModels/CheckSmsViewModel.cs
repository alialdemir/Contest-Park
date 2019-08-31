using Acr.UserDialogs;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CheckSmsViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly ISettingsService _settingsService;

        #endregion Private variables

        #region Constructor

        public CheckSmsViewModel(IPopupNavigation popupNavigation,
                                 IIdentityService identityService,
                                 INavigationService navigationService,
                                 ISettingsService settingsService) : base(navigationService: navigationService, popupNavigation: popupNavigation)
        {
            _identityService = identityService;
            _settingsService = settingsService;
        }

        #endregion Constructor

        #region Properties

        public string PhoneNumber { get; set; }

        public int SmsCode { get; set; }

        public int? UserSmsCode { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Telefon numarası kayıtlı mı diye kontrol eder eğer kayıtlı ise login yapar
        /// eğer kayıtlı değilse kayıt olma popupu başlangıcı olan
        /// </summary>
        private async Task ExecuteCheckSmsCodeCommand()
        {
            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            // TODO: check sms

            string userName = await _identityService.GetUserNameByPhoneNumber(PhoneNumber);
            if (!string.IsNullOrEmpty(userName))
            {
                await SignInAsync(userName);

                UserDialogs.Instance.HideLoading();

                return;
            }

            UserDialogs.Instance.HideLoading();

            await RemoveFirstPopupAsync();

            await PushPopupPageAsync(new SignUpFullNameView()
            {
                PhoneNumber = PhoneNumber
            });
        }

        /// <summary>
        /// Giriş yap
        /// </summary>
        private async Task SignInAsync(string userName)
        {
            UserToken token = await _identityService.GetTokenAsync(new Models.LoginModel
            {
                Password = PhoneNumber,
                UserName = userName
            });
            if (token != null)
            {
                _settingsService.SetTokenInfo(token);

                await PushNavigationPageAsync($"app:///{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}?appModuleRefresh=OnInitialized");
            }

            UserDialogs.Instance.HideLoading();
        }

        #endregion Methods

        #region Commands

        public ICommand CheckSmsCodeCommand => new Command(async () => await ExecuteCheckSmsCodeCommand());

        #endregion Commands
    }
}
