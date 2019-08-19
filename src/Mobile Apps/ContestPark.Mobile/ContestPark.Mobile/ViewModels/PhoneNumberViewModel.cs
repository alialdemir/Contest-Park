using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PhoneNumberViewModel : BindableBase
    {
        #region Constructor

        public PhoneNumberViewModel()
        {
        }

        #endregion Constructor

        #region Commands
        public ICommand SignInCommand => new Command(async () => await ExecuteSignInCommandAsync());

        private Task ExecuteSignInCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
