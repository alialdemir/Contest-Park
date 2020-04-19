using Android.App;
using Android.Runtime;
using ContestPark.Mobile.Services.Shiny;
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
            base.OnCreate();
            AndroidShinyHost.Init(this, new ShinyAppStartup());
        }
    }
}
