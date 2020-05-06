//using ContestPark.Mobile.Services.Shiny;
//using ContestPark.Mobile.Services.Shiny;
using FFImageLoading.Forms.Platform;

//using Firebase.Core;

//using Firebase.Core;
using Foundation;

//using Google.MobileAds;
using Plugin.Segmented.Control.iOS;
using Prism;
using Prism.Ioc;
using Rg.Plugins.Popup;

//using Shiny;
using System;
using System.IO;
using System.Text;
using UIKit;

namespace ContestPark.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    Shiny.Jobs.JobManager.OnBackgroundFetch(completionHandler);
        //}

        //public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        //    => this.ShinyDidReceiveRemoteNotification(userInfo, completionHandler);

        //public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        //    => this.ShinyRegisteredForRemoteNotifications(deviceToken);

        //public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        //    => this.ShinyFailedToRegisterForRemoteNotifications(error);

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
#if !DEBUG
            CheckJailBreak();
#endif

            CachedImageRenderer.Init();

            Popup.Init();

            //App.Configure();

            //MobileAds.SharedInstance.Start(status =>
            //{
            //    // Requests test ads on devices you specify. Your test device ID is printed to the console when
            //    // an ad request is made. Ads automatically returns test ads when running on a
            //    // simulator. After you get your device ID, add it here
            //    MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = new[] { Request.SimulatorId.ToString() };
            //});

            //   iOSShinyHost.Init(new ShinyAppStartup());

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            SegmentedControlRenderer.Initialize();

            UIApplication.SharedApplication.IdleTimerDisabled = true;


            LoadApplication(new ContestParkApp(new IOSInitializer()));

            return base.FinishedLaunching(app, options);
        }

        private bool CheckCydia()
        {
            string[] cydiaPaths = new string[]
            {
                    @"/Applications/Cydia.app",
                    @"/Library/MobileSubstrate/MobileSubstrate.dylib",
                    @"/bin/bash",
                    @"/usr/sbin/sshd",
                    @"/etc/apt"
            };

            foreach (var item in cydiaPaths)
            {
                NSString filePath = new NSString(item);
                if (File.Exists(filePath))
                    return true;
            }

            return false;
        }

        private bool WriteToJailBreak()
        {
            try
            {
                NSString testString = new NSString("This is a test");
                var filename = @"/private/jailbreak.txt";

                File.WriteAllText(filename, testString, Encoding.UTF8);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckCydiaPath()
        {
            try
            {
                return UIApplication.SharedApplication.OpenUrl(
                new NSUrl(@"cydia://package/com.example.package"));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void CheckJailBreak()
        {
            if (CheckCydia() || WriteToJailBreak() || CheckCydiaPath())
                throw new Exception("JailBreak!");
        }
    }

    public class IOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
