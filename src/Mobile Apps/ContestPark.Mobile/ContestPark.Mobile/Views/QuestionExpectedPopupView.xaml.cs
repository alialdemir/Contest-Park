using Rg.Plugins.Popup.Pages;

namespace ContestPark.Mobile.Views
{
    public partial class QuestionExpectedPopupView : PopupPage
    {
        #region Constructor

        public QuestionExpectedPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion Methods
    }
}
