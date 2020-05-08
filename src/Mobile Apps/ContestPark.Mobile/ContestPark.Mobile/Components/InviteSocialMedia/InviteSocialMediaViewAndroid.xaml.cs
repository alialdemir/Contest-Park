using ContestPark.Mobile.Models.InviteSocialMedia;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.InviteSocialMedia
{
    public partial class InviteSocialMediaViewAndroid : ContentView
    {
        #region Constructor

        public InviteSocialMediaViewAndroid()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        public InviteSocialMediaModel Data
        {
            set
            {
                lblUserName.Text = value.UserName;
            }
        }

        #endregion Methods
    }
}
