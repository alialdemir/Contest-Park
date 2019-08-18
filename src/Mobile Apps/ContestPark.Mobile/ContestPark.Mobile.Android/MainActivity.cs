using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using ImageCircle.Forms.Plugin.Droid;
using Lottie.Forms.Droid;
using Plugin.CurrentActivity;
using Plugin.Iconize;
using Plugin.InAppBilling;
using Prism;
using Prism.Ioc;
using System;
using Xamarin.Forms.PancakeView.Droid;

namespace ContestPark.Mobile.Droid
{
    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register any platform specific implementations
        }
    }

    [Activity(Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Toolbar ToolBar { get; private set; }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //   ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            base.OnCreate(bundle);

#if !DEBUG
            CheckForRoot();
#endif
            // Check if running in sim

            CrossCurrentActivity.Current.Init(this, bundle);

            CrossCurrentActivity.Current.Activity = this;

            UserDialogs.Init(this);

            ImageCircleRenderer.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            Xamarin.Essentials.Platform.Init(this, bundle);

            global::Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.SetFlags("Visual_Experimental"); // ONLY if using a pre-release of Xamarin.Forms

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Iconize.Init(Resource.Id.toolbar, Resource.Id.sliding_tabs);

            Xamarin.Forms.FormsMaterial.Init(this, bundle);

            AnimationViewRenderer.Init();

            LoadApplication(new ContestParkApp(new AndroidInitializer()));

            PancakeViewRenderer.Init();

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
                //Eğer rootlu bir cihaz uygulamanızı yüklediyse, uygulamanızı kapatabilirsiniz.
                Process.KillProcess(Process.MyPid());
            }
        }

        #endregion Security
    }
}
