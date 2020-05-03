using ContestPark.Mobile.Models.InviteSocialMedia;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.InviteSocialMedia
{
    
    public partial class InviteSocialMediaView : ContentView
    {
        #region Constructor

        public InviteSocialMediaView()
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
