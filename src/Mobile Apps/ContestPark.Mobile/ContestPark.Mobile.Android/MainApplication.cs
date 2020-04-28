using Android.App;
using Android.Runtime;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Services.Shiny;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Shiny;
using System;

namespace ContestPark.Mobile.Droid
{
    [Application(Label = "ContestPark")]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            AppCenter.Start(GlobalSetting.AppCenterKey, typeof(Crashes));

            base.OnCreate();
            AndroidShinyHost.Init(this, new ShinyAppStartup());
        }
    }
}
