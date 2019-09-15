using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpUserNameView : PopupPage
    {
        #region Constructor

        public SignUpUserNameView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((SignUpUserNameViewModel)BindingContext);

            if (viewModel == null && !viewModel.IsInitialized)
                return;

            viewModel.FullName = FullName;
            viewModel.PhoneNumber = PhoneNumber;
        }

        #endregion Methods
    }
}
