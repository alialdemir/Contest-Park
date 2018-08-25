using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionExpectedPopupView : PopupPage
    {
        #region Constructor

        public QuestionExpectedPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string SubcategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        public byte RoundCount { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((QuestionExpectedPopupViewModel)BindingContext);
            if (viewModel == null)
                return;
            viewModel.SubcategoryName = SubcategoryName;
            viewModel.SubCategoryPicturePath = SubCategoryPicturePath;
            viewModel.RoundCount = RoundCount;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion Methods
    }
}