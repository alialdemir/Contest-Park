using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhoneNumberView : ContentPage
    {
        #region Constructor

        public PhoneNumberView()
        {
            InitializeComponent();
            BaseNavigationPage.SetHasNavigationBar(this, false);
        }

        #endregion Constructor
    }
}
