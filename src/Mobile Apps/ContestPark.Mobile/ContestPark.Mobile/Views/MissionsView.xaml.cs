using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MissionsView : ContentPage
    {
        #region Constructor

        public MissionsView()
        {
            InitializeComponent();
            
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            MissionsViewModel viewModel = ((MissionsViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;

            base.OnAppearing();
        }

        #endregion Methods
    }
}
