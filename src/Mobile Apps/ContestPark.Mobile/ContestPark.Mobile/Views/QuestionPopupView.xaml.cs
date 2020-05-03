using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace ContestPark.Mobile.Views
{
    public partial class QuestionPopupView : PopupPage
    {
        #region Constructor

        public QuestionPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((QuestionPopupViewModel)BindingContext);
            if (viewModel == null || viewModel.IsInitialized)
                return;

            //viewModel.AnimateStylishCommand = new Command(Stylishs.AnimateStylish);
        }

        protected override bool OnBackButtonPressed()
        {
            ((QuestionPopupViewModel)BindingContext).GotoBackCommand.Execute(true);
            return true;
        }

        #endregion Methods
    }
}
