using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
            ((QuestionPopupViewModel)BindingContext).DuelCloseCommand.Execute(true);
            return true;
        }

        #endregion Methods
    }
}
