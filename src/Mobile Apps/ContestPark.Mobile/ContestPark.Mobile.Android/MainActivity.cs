using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using ContestPark.Mobile.Configs;
using Lottie.Forms.Droid;
using Matcha.BackgroundService.Droid;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using Plugin.InAppBilling;
using Prism;
using Prism.Ioc;
using Xamarin.Forms.PancakeView.Droid;

namespace ContestPark.Mobile.Droid
{
    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register any platform specific implementations
        }
    }

    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Toolbar ToolBar { get; private set; }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            BackgroundAggregator.Init(this);

            base.OnCreate(bundle);

            // Check if running in sim

            CrossCurrentActivity.Current.Init(this, bundle);

            CrossCurrentActivity.Current.Activity = this;

            UserDialogs.Init(this);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, GlobalSetting.AppUnitId);

            Xamarin.Essentials.Platform.Init(this, bundle);

            global::Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.SetFlags("Visual_Experimental"); // ONLY if using a pre-release of Xamarin.Forms

            global::Xamarin.Forms.Forms.Init(this, bundle);

            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            AnimationViewRenderer.Init();

            LoadApplication(new ContestParkApp(new AndroidInitializer()));

            FirebasePushNotificationManager.ProcessIntent(this, Intent);

            PancakeViewRenderer.Init();

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);// Uygulama kilit ekranına düşmemesi için(rakip aranıyor ve düello ekranlarında kilit ekranına düşerse yenilmiş saymaması için ekledim
        }
    }
}
