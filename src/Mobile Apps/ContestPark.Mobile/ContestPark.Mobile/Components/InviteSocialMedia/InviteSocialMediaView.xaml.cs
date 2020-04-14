using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.InviteSocialMedia;
using FFImageLoading.Transformations;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.InviteSocialMedia
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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

                imgProfilePicture.Source = GetImage(value.ProfilePicturePath);


                imgProfilePicture.Transformations.Add(new CircleTransformation(20, "#ffc107"));

                imgProfilePicture.HeightRequest = imgProfilePicture.WidthRequest = 40;
            }
        }

        /// <summary>
        /// Eğer url ise fromUrl değilse fromFile ile resmi yükler
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private ImageSource GetImage(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return FileImageSource.FromFile(DefaultImages.DefaultProfilePicture);

            return uri.StartsWith("http") ?
                FileImageSource.FromUri(new Uri(uri)) :
                FileImageSource.FromFile(uri);
        }

        #endregion Methods
    }
}
