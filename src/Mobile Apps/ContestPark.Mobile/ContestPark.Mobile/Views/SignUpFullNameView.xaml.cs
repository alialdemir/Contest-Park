using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpFullNameView : PopupPage
    {
        #region Constructor

        public SignUpFullNameView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string PhoneNumber { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((SignUpFullNameViewModel)BindingContext);

            if (viewModel == null && !viewModel.IsInitialized)
                return;

            viewModel.PhoneNumber = PhoneNumber;
        }

        #endregion Methods
    }
}
