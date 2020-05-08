using ContestPark.Mobile.Models.InviteSocialMedia;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.InviteSocialMedia
{
    public partial class InviteSocialMediaViewIos : ContentView
    {
        #region Constructor

        public InviteSocialMediaViewIos()
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
