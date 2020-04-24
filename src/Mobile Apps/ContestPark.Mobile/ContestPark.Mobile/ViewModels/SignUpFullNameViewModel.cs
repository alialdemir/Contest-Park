using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContestPark.Mobile.ViewModels
{
    public class SignUpFullNameViewModel : ViewModelBase
    {
        #region Constructor

        public SignUpFullNameViewModel(INavigationService navigationService,
                                       IPageDialogService dialogService) : base(navigationService: navigationService,
                                                                                dialogService: dialogService)
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

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("PhoneNumber"))
                PhoneNumber = parameters.GetValue<string>("PhoneNumber");

            return base.InitializeAsync(parameters);
        }

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

            GotoBackCommand.Execute(true);

            await NavigateToPopupAsync<SignUpUserNameView>(new NavigationParameters
            {
                { "FullName", FullName },
                { "PhoneNumber", PhoneNumber },
            });

            IsBusy = false;
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            return base.GoBackAsync(parameters, useModalNavigation: true);
        }

        #endregion Methods

        #region Commands

        public ICommand FullNameCommand => new CommandAsync(ExecuteFullNameCommand);

        #endregion Commands
    }
}
