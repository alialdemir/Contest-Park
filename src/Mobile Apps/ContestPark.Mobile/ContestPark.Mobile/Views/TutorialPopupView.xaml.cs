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
    }
}
