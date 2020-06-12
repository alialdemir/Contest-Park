using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using ContestPark.Mobile.Configs;
using Lottie.Forms.Droid;
using Plugin.CurrentActivity;
using Plugin.InAppBilling;
using Prism;
using Prism.Ioc;
using System.Collections.Generic;

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

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);

            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            PushNotification();

            // Check if running in sim
            CrossCurrentActivity.Current.Init(this, bundle);

            CrossCurrentActivity.Current.Activity = this;

            UserDialogs.Init(this);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            MobileAds.Initialize(ApplicationContext, GlobalSetting.AppUnitId);

            Xamarin.Essentials.Platform.Init(this, bundle);

            global::Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            AnimationViewRenderer.Init();

            LoadApplication(new ContestParkApp(new AndroidInitializer()));

            PancakeViewRenderer.Init();

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);// Uygulama kilit ekranına düşmemesi için(rakip aranıyor ve düello ekranlarında kilit ekranına düşerse yenilmiş saymaması için ekledim
        }

        private void PushNotification()
        {
            //Remove this method to stop OneSignal Debugging
            OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.NONE);

            OneSignal.Current.StartInit(GlobalSetting.OneSignalAppId)
                             .Settings(new Dictionary<string, bool>()
                             {
                                 { IOSSettings.kOSSettingsKeyAutoPrompt, false },
                                 { IOSSettings.kOSSettingsKeyInAppLaunchURL, false }
                             })
                            .EndInit();

            // The promptForPushNotificationsWithUserResponse function will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 7)
            OneSignal.Current.RegisterForPushNotifications();
        }
    }
}
