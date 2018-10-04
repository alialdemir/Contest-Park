using ContestPark.Mobile.Configs;
using System;
using Xamarin.Forms;

namespace ContestPark.Mobile.Converters.SignInSocialNetworkPage
{
    public class SignInSocialNetworkPage : ContentPage
    {
        #region Enum

        public enum SocialNetworkTypes : byte
        {
            Facebook = 1,
            Twitter = 2,
            GooglePlus = 3
        }

        #endregion Enum

        #region Constructor

        public SignInSocialNetworkPage(SocialNetworkTypes socialNetworkType)
        {
            if (!SocialNetworkTypes.Facebook.HasFlag(socialNetworkType) ||
                !SocialNetworkTypes.Twitter.HasFlag(socialNetworkType) ||
                !SocialNetworkTypes.GooglePlus.HasFlag(socialNetworkType)) throw new ArgumentNullException(nameof(socialNetworkType));
            CreateSocialInfo(socialNetworkType);
        }

        #endregion Constructor

        #region Properties

        public string ClientId { get; private set; }

        public string AuthorizeUrl { get; private set; }

        public string RedirectUrl { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gelen sosyal ağ tipine göre bilgileri set eder
        /// </summary>
        /// <param name="socialNetworkType">Hangi sosyal ağ ile bağlandığı</param>
        private void CreateSocialInfo(SocialNetworkTypes socialNetworkType)
        {
            switch (socialNetworkType)
            {
                case SocialNetworkTypes.Facebook:
                    ClientId = OAuthConfig.FacebookClientId;
                    AuthorizeUrl = OAuthConfig.FacebookAuthorizeUrl;
                    RedirectUrl = OAuthConfig.FacebookRedirectUrl;
                    break;

                default:
                    break;
            }
        }

        #endregion Methods

        #region Command

        public Command<string> CompletedCommand { get; set; }
        public Command<string> ErrorCommand { get; set; }

        #endregion Command
    }
}