using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContestStoreView : ContentPage
    {
        #region Constructor

        public ContestStoreView()
        {
            InitializeComponent();
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IconOverride = ImageSource.FromResource("menuicon.png") });
        }

        #endregion Constructor

        #region Properties

        private ContestStoreViewModel _viewModel;

        public ContestStoreViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ((ContestStoreViewModel)BindingContext);
                }

                return _viewModel;
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            if (ViewModel == null || ViewModel.IsInitialized)
                return;

            ViewModel.InitializeCommand.Execute(null);
            ViewModel.IsInitialized = true;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ViewModel.RemoveOnRewardedVideoAdClosed.Execute(null);

            base.OnDisappearing();
        }

        #endregion Methods
    }
}
