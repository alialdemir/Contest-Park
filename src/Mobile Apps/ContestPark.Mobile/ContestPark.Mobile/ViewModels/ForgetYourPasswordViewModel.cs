using Acr.UserDialogs;
using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class ForgetYourPasswordViewModel : ViewModelBase
    {
        #region Private variables

        private readonly IIdentityService _identityService;

        #endregion Private variables

        #region Constructor

        public ForgetYourPasswordViewModel(IIdentityService identityService)
        {
            Title = ContestParkResources.ForgotYourPassword;
            _identityService = identityService;
        }

        #endregion Constructor

        #region Properties

        private string _userNameOrEmailAddress;

        public string UserNameOrEmailAddress
        {
            get
            {
                return _userNameOrEmailAddress;
            }
            set
            {
                _userNameOrEmailAddress = value;
                RaisePropertyChanged(() => UserNameOrEmailAddress);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Şifremi unuttum execute method
        /// </summary>
        private async Task ExecuteForgetYourPasswordCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            UserDialogs.Instance.ShowLoading("", MaskType.Black);

            await _identityService.ForgetYourPasswordAsync(UserNameOrEmailAddress);

            UserDialogs.Instance.HideLoading();

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ForgetYourPasswordCommand => new Command(async () => await ExecuteForgetYourPasswordCommand());

        #endregion Commands
    }
}