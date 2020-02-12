using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialPopupView : PopupPage
    {
        #region Constructor

        public TutorialPopupView()
        {
            InitializeComponent();
            BaseNavigationPage.SetHasNavigationBar(this, false);
        }

        #endregion Constructor

        #region Override

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TutorialPopupViewModel viewModel = ((TutorialPopupViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        #endregion Override
    }
}
