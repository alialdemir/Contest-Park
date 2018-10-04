using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInView : ContentPage
    {
        #region Constructor

        public SignInView()
        {
            InitializeComponent();
            BaseNavigationPage.SetHasNavigationBar(this, false);
        }

        #endregion Constructor
    }
}