using ContestPark.Mobile.Dependencies;
using UIKit;

[assembly: Xamarin.Forms.DependencyAttribute(typeof(ContestPark.Mobile.iOS.Dependencies.DeviceHelper))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class DeviceHelper : IDevice
    {
        public string GetIdentifier()
        {
            string serial = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            return serial;
        }

        public Models.DeviceHelper.DeviceHelper GeScreenSize()
        {
            return new Models.DeviceHelper.DeviceHelper();
        }
    }
}
