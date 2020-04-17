using FFImageLoading.Forms.Platform;
using Firebase.Core;
using Foundation;

//using Google.MobileAds;
using Matcha.BackgroundService.iOS;
using Plugin.FirebasePushNotification;
using Plugin.Segmented.Control.iOS;
using Prism;
using Prism.Ioc;
using Rg.Plugins.Popup;
using System;
using System.Collections.Generic;
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
        //
        // This method is invoked when the application has loaded and is ready to run. In this"
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }

        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method.
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
#if !DEBUG
                        CheckJailBreak();
#endif

            BackgroundAggregator.Init(this);

            CachedImageRenderer.Init();

            Popup.Init();

            App.Configure();

            //MobileAds.SharedInstance.Start(status =>
            //{
            //    // Requests test ads on devices you specify. Your test device ID is printed to the console when
            //    // an ad request is made. Ads automatically returns test ads when running on a
            //    // simulator. After you get your device ID, add it here
            //    MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = new[] { Request.SimulatorId.ToString() };
            //});

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            SegmentedControlRenderer.Initialize();

            LoadApplication(new ContestParkApp(new IOSInitializer()));

            FirebasePushNotificationManager.Initialize(options, new NotificationUserCategory[]
         {
                new NotificationUserCategory("message",new List<NotificationUserAction> {
                    new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground)
                }),
                new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept"),
                    new NotificationUserAction("Reject","Reject",NotificationActionType.Destructive)
                })
         });

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
