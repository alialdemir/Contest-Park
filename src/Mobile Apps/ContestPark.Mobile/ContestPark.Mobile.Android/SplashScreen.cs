using Android.App;
using Android.Content.PM;
using Android.OS;
using System;

namespace ContestPark.Mobile.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //#if !DEBUG burası sanırım arm7a işlemcilerde hataya sebep oluyor
            //                CheckForRoot();
            //#endif

            this.StartActivity(typeof(MainActivity));
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
    }
}
