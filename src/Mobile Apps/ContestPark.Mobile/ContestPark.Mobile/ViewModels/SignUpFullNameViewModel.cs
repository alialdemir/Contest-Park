using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpFullNameViewModel : ViewModelBase
    {
        #region Constructor

        public SignUpFullNameViewModel(IPopupNavigation popupNavigation,
                                       IPageDialogService dialogService) : base(dialogService: dialogService,
                                                                                popupNavigation: popupNavigation)
        {
        }

        #endregion Constructor

        #region Properties

        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                RaisePropertyChanged(() => FullName);
            }
        }

        public string PhoneNumber { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ad soyad bilgisini alır bir sonraki popupa yönlendirir
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteFullNameCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (string.IsNullOrEmpty(FullName))// Ad soyad boş ise
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.FullNameRequiredFields,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }
            else if (FullName.Length < 3)// Ad soyad 3 karakterden küçük olamaz
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.FullNameMinLength,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }
            else if (FullName.Length > 255)// Ad soyad 255 karakterden büyük olamaz
            {
                await DisplayAlertAsync(ContestParkResources.Error,
                                        ContestParkResources.FullNameMaxLength,
                                        ContestParkResources.Okay);
                IsBusy = false;

                return;
            }

            await RemoveFirstPopupAsync();

            await PushPopupPageAsync(new SignUpUserNameView()
            {
                FullName = FullName,
                PhoneNumber = PhoneNumber
            });

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        public ICommand ClosePopupCommand { get { return new Command(async () => await RemoveFirstPopupAsync()); } }
        public ICommand FullNameCommand => new Command(async () => await ExecuteFullNameCommand());

        #endregion Commands
    }
}
