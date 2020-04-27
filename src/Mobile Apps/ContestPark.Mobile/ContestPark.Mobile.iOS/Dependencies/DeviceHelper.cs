using ContestPark.Mobile.Dependencies;
using System.Diagnostics;
using UIKit;

[assembly: Xamarin.Forms.DependencyAttribute(typeof(ContestPark.Mobile.iOS.Dependencies.DeviceHelper))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class DeviceHelper : IDevice
    {
        public void CloseApp()
        {
            Process.GetCurrentProcess().CloseMainWindow();
            Process.GetCurrentProcess().Close();
        }

        public string GetIdentifier()
        {
            string serial = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            return serial;
        }

        public void DismissKeyboard()
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var window = UIApplication.SharedApplication.KeyWindow;
                var vc = window.RootViewController;
                while (vc.PresentedViewController != null)
                {
                    vc = vc.PresentedViewController;
                }

                vc.View.EndEditing(true);
            });
        }
    }
}
