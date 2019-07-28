using Foundation;
using Prism;
using Prism.Ioc;
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
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //      XfxControls.Init();
            //    UserDialogs.Init(x=> { });
            //    ImageCircleRenderer.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            CheckJailBreak();

            //   Rg.Plugins.Popup.Popup.Init();

            global::Xamarin.Forms.Forms.Init();
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
