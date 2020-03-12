using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class NotificationView : ContentPage
    {
        #region Constructor

        public NotificationView()
        {
            InitializeComponent();

            Shell.SetTabBarIsVisible(this, false);// Altta tabbar gözükmemesi için ekledim
        }

        #endregion Constructor
    }
}
