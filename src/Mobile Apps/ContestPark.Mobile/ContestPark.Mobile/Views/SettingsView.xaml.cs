using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class SettingsView : ContentPage
    {
        #region Constructors

        public SettingsView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        protected override void OnAppearing()
        {
            SettingsViewModel viewModel = ((SettingsViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;

            base.OnAppearing();
        }

        #endregion Methods
    }
}
