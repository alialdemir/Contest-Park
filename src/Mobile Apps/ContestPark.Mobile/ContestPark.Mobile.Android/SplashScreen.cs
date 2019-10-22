using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using Lottie.Forms.Droid;
using Plugin.CurrentActivity;
using Plugin.Iconize;
using Xamarin.Forms.PancakeView.Droid;

namespace ContestPark.Mobile.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.StartActivity(typeof(MainActivity));

            // Check if running in sim

            CrossCurrentActivity.Current.Init(this, bundle);

            CrossCurrentActivity.Current.Activity = this;

            UserDialogs.Init(this);

            ImageCircleRenderer.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            Xamarin.Essentials.Platform.Init(this, bundle);

            global::Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.SetFlags("Visual_Experimental"); // ONLY if using a pre-release of Xamarin.Forms

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Iconize.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);

            Xamarin.Forms.FormsMaterial.Init(this, bundle);

            AnimationViewRenderer.Init();

            PancakeViewRenderer.Init();

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);// Uygulama kilit ekranına düşmemesi için(rakip aranıyor ve düello ekranlarında kilit ekranına düşerse yenilmiş saymaması için ekledim
        }
    }
}
