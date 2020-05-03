using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WinningsView : ContentPage
    {
        #region Constructor

        public WinningsView()
        {
            InitializeComponent();
            
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            WinningsViewModel viewModel = ((WinningsViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;

            base.OnAppearing();
        }

        #endregion Methods
    }
}
