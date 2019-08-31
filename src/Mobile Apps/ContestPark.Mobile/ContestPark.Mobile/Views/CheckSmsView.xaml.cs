using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckSmsView : PopupPage
    {
        #region Constructor

        public CheckSmsView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string PhoneNumber { get; set; }
        public int SmsCode { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((CheckSmsViewModel)BindingContext);

            if (viewModel == null && !viewModel.IsInitialized)
                return;

            viewModel.PhoneNumber = PhoneNumber;
            viewModel.SmsCode = SmsCode;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion Methods
    }
}
