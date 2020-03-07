using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace ContestPark.Mobile.Views
{
    public partial class SelectSubCategoryView : PopupPage
    {
        #region Constructor

        public SelectSubCategoryView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string OpponentUserId { get; set; }

        #endregion Properties

        #region Overrides

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SelectSubCategoryViewModel viewModel = (SelectSubCategoryViewModel)BindingContext;
            if (viewModel == null)
                return;

            viewModel.OpponentUserId = OpponentUserId;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        #endregion Overrides
    }
}
