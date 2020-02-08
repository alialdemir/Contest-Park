using Android.App;
using Android.Content.PM;
using Android.OS;
using ContestPark.Mobile.AppResources;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (CheckNetworkAsync())
                return;

            this.StartActivity(typeof(MainActivity));
        }

        /// <summary>
        /// Uygulama ilk açıldığında internet var mı diye kontrol eder
        /// </summary>
        /// <returns>İnternet yoksa false varsa true</returns>
        private bool CheckNetworkAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetMessage(ContestParkResources.NoInternet);

                alert.SetPositiveButton(ContestParkResources.Okay, (senderAlert, args) =>
                {
                    Process.KillProcess(Process.MyPid());
                });

                Dialog dialog = alert.Create();
                dialog.Show();

                return true;
            }

            return false;
        }
    }
}
