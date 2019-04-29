using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgetYourPasswordView : ContentPage
    {
        #region Constructor

        public ForgetYourPasswordView()
        {
            InitializeComponent();

            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = (Color)ContestParkApp.Current.Resources["Primary"];
            ((NavigationPage)Application.Current.MainPage).BarTextColor = (Color)ContestParkApp.Current.Resources["Black"];
        }

        #endregion Constructor
    }
}