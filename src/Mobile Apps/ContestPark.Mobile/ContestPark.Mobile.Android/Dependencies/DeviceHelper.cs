using Android.App;
using Android.Content.Res;
using Android.Provider;
using Android.Views.InputMethods;
using ContestPark.Mobile.Dependencies;

[assembly: Xamarin.Forms.Dependency(typeof(ContestPark.Mobile.Droid.Dependencies.DeviceHelper))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class DeviceHelper : IDevice
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public string GetIdentifier()
        {
            return Settings.Secure.GetString(Application.Context.ApplicationContext.ContentResolver,
            Settings.Secure.AndroidId);
        }

        public void DismissKeyboard()
        {
            var inputMethodManager = InputMethodManager.FromContext(Application.Context);
            if (inputMethodManager != null)
            {
                inputMethodManager.HideSoftInputFromWindow(
                    ((Activity)Xamarin.Forms.Forms.Context).Window.DecorView.WindowToken, HideSoftInputFlags.NotAlways);
            }
        }
    }
}
