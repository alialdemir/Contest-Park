using Android.App;
using Android.Provider;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Droid.Dependencies;

[assembly: Xamarin.Forms.Dependency(typeof(UniqueIdAndroid))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class UniqueIdAndroid : IDevice
    {
        public string GetIdentifier()
        {
            return Settings.Secure.GetString(Application.Context.ApplicationContext.ContentResolver,
            Settings.Secure.AndroidId);
        }
    }
}
