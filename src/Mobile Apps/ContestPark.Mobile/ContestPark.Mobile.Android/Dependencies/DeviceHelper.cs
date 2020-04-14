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
        public Models.DeviceHelper.DeviceHelper GeScreenSize()
        {
            var metrics = Resources.System.DisplayMetrics;
            return new Models.DeviceHelper.DeviceHelper
            {
                ScreenHeight = ConvertToPixelsToDp(metrics.HeightPixels),
                ScreenWidth = ConvertToPixelsToDp(metrics.WidthPixels)
            };
        }

        private int ConvertToPixelsToDp(int pixelValue)
        {
            return (int)(((int)pixelValue) / Resources.System.DisplayMetrics.Density);
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
