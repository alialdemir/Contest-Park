using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Login;
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
        #region Constructor

        public SignUpUserNameViewModel(INavigationService navigationService,
                                       IPageDialogService dialogService) : base(navigationService: navigationService,
                                                                                dialogService: dialogService)
        {
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

        private string PhoneNumber { get; set; }
        private string FullName { get; set; }
        private string ReferenceCode { get; set; }

        #endregion Properties

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("FullName"))
                FullName = parameters.GetValue<string>("FullName");

            if (parameters.ContainsKey("PhoneNumber"))
                PhoneNumber = parameters.GetValue<string>("PhoneNumber");

            if (parameters.ContainsKey("ReferenceCode"))
                ReferenceCode = parameters.GetValue<string>("ReferenceCode");

            return base.InitializeAsync(parameters);
        }

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

            GotoBackCommand.Execute(true);

            await NavigateToPopupAsync<SignUpSelectSubCategoriesView>(new NavigationParameters
            {
                {
                    "SignUp", new SignUpModel
                                        {
                                            FullName = FullName?.Trim(),
                                            Password = PhoneNumber,
                                            UserName = UserName?.Trim(),
                                            ReferenceCode = ReferenceCode?.Trim()
                                        }
                }
            });

            IsBusy = false;
        }

        /// <summary>
        /// Referans kodu girme sayfasına yönlendirir
        /// </summary>
        private void ExecuteGotoReferenceCodeCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            NavigateToPopupAsync<SignUpReferenceCodeView>();

            IsBusy = false;
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            return base.GoBackAsync(parameters, useModalNavigation: true);
        }

        #endregion Methods

        #region Commands

        public ICommand UserNameCommand => new CommandAsync(ExecuteUserNameCommandAsync);
        public ICommand GotoReferenceCodeCommand => new Command(ExecuteGotoReferenceCodeCommand);

        #endregion Commands
    }
}
