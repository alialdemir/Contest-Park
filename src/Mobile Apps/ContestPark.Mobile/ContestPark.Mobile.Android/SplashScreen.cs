using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ContestPark.Mobile.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //  System.Threading.Thread.Sleep(2000);
            this.StartActivity(typeof(MainActivity));
        }
    }
}