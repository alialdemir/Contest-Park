using FFImageLoading.Forms.Platform;
using Foundation;

//using Google.MobileAds;
using ImageCircle.Forms.Plugin.iOS;
using Plugin.Segmented.Control.iOS;
using Prism;
using Prism.Ioc;
using Rg.Plugins.Popup;
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
        //
        // This method is invoked when the application has loaded and is ready to run. In this"
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //#if !DEBUG
            //            CheckJailBreak();
            //#endif

            CachedImageRenderer.Init();
            //UserDialogs.Init();

            ImageCircleRenderer.Init();

            Popup.Init();

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            SegmentedControlRenderer.Initialize();

            //   MobileAds.SharedInstance.Start(CompletionHandler);

            LoadApplication(new ContestParkApp(new IOSInitializer()));

            Firebase.Core.App.Configure();

            return base.FinishedLaunching(app, options);
        }

        /// <summary>
        /// Admob için eklendi
        /// </summary>
        //private void CompletionHandler(InitializationStatus status) { }

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
            catch (Exception ex)
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
            catch (Exception ex)
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
