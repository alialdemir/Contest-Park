using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpView : ContentPage
    {
        #region Constructor

        public SignUpView()
        {
            InitializeComponent();

            ((BaseNavigationPage)Application.Current.MainPage).BarBackgroundColor = (Color)ContestParkApp.Current.Resources["Primary"];
            ((BaseNavigationPage)Application.Current.MainPage).BarTextColor = (Color)ContestParkApp.Current.Resources["Black"];
        }

        #endregion Constructor
    }
}