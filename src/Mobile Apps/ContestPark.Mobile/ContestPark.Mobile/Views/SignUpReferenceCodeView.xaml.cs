using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpReferenceCodeView : PopupPage
    {
        #region Constructor

        public SignUpReferenceCodeView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((SignUpReferenceCodeViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.ReferenceCodePopup = this;
        }

        #endregion Methods
    }
}
