using Plugin.Iconize;
using Prism.Navigation;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseNavigationPage : IconNavigationPage, INavigationPageOptions
    {
        #region Constructor

        public BaseNavigationPage()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Property

        public bool ClearNavigationStackOnNavigation
        {
            get { return true; }
        }

        #endregion Property
    }
}
