using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentPage
    {
        #region Constructor

        public MainView()
        {
            InitializeComponent();

            BackgroundColor = (Color)ContestParkApp.Current.Resources["Primary"];
        }

        #endregion Constructor
    }
}