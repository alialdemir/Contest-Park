//using Android.App;
//using ContestPark.Mobile.Converters.SignInSocialNetworkPage;
//using ContestPark.Mobile.Droid.CustomRenderer;
//using System;

////using Xamarin.Auth;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(SignInSocialNetworkPage), typeof(SignInSocialNetworkPageRenderer))]

//namespace ContestPark.Mobile.Droid.CustomRenderer
//{
//    public class SignInSocialNetworkPageRenderer : PageRenderer
//    {
//        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
//        {
//            base.OnElementChanged(e);
//            var socialNetworks = (SignInSocialNetworkPage)e.NewElement;

//            #region Validations

//            if (String.IsNullOrEmpty(socialNetworks.ClientId)) throw new ArgumentNullException(nameof(socialNetworks.ClientId));
//            if (String.IsNullOrEmpty(socialNetworks.AuthorizeUrl)) throw new ArgumentNullException(nameof(socialNetworks.AuthorizeUrl));
//            if (String.IsNullOrEmpty(socialNetworks.RedirectUrl)) throw new ArgumentNullException(nameof(socialNetworks.RedirectUrl));
//            if (socialNetworks.CompletedCommand == null) throw new ArgumentNullException(nameof(socialNetworks.CompletedCommand));
//            if (socialNetworks.ErrorCommand == null) throw new ArgumentNullException(nameof(socialNetworks.ErrorCommand));

//            #endregion Validations

//            var activity = this.Context as Activity;
//            //var auth = new OAuth2Authenticator(
//            //    clientId: socialNetworks.ClientId, // your OAuth2 client id
//            //    scope: "", // The scopes for the particular API you're accessing. The format for this will vary by API.
//            //    authorizeUrl: new Uri(socialNetworks.AuthorizeUrl), // the auth URL for the service
//            //    redirectUrl: new Uri(socialNetworks.RedirectUrl)); // the redirect URL for the service

//            //auth.Completed += (sender, eventArgs) =>
//            //{
//            //    if (eventArgs.IsAuthenticated)
//            //    {
//            //        string accessToken = eventArgs.Account.Properties["access_token"];
//            //        socialNetworks.CompletedCommand.Execute(accessToken);
//            //    }
//            //    else
//            //    {
//            //        socialNetworks.ErrorCommand.Execute("Cancal");
//            //    }
//            //};
//            //auth.Error += (sender, eventArgs) => socialNetworks.ErrorCommand.Execute(eventArgs.Message);
//            //activity.StartActivity(auth.GetUI(activity));
//        }
//    }
//}