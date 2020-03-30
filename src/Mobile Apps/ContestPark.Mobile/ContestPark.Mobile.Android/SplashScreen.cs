using Android.App;
using Android.Content.PM;
using Android.OS;
using ContestPark.Mobile.AppResources;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);

#if !DEBUG
                CheckForRoot();
#endif

                if (CheckNetworkAsync())
                    return;

                this.StartActivity(typeof(MainActivity));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        #region Security

        private bool CanExecuteSuCommand()
        {
            try
            {
                Java.Lang.Runtime.GetRuntime().Exec("su");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

                return false;
            }
        }

        private bool HasSuperApk()
        {
            return new Java.IO.File("/system/app/Superuser.apk").Exists();
        }

        private bool IsTestKeyBuild()
        {
            string str = Build.Tags;
            if ((str != null) && (str.Contains("test-keys")))
                return true;
            return false;
        }

        private void CheckForRoot()
        {
            if (CanExecuteSuCommand() || HasSuperApk() || IsTestKeyBuild())
            {
                Process.KillProcess(Process.MyPid());
            }
        }

        #endregion Security

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
