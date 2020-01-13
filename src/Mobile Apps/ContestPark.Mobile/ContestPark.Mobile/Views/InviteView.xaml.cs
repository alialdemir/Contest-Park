using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InviteView : ContentPage
    {
        #region Constructor

        public InviteView()
        {
            InitializeComponent();
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IconOverride = ImageSource.FromFile("menuicon.png") });
        }

        #endregion Constructor
    }
}
