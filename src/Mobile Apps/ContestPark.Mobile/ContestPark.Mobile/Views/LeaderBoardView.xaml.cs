using ContestPark.Mobile.ViewModels.Base;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class LeaderBoardView : ContentPage
    {
        #region Constructor

        public LeaderBoardView()
        {
            InitializeComponent();
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IconOverride = ImageSource.FromResource("menuicon.png") });
        }

        #endregion Constructor

        #region Override

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModelBase viewModel = ((ViewModelBase)BindingContext);
            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        #endregion Override
    }
}
