using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ContestPark.Mobile.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            CrossCurrentActivity.Current.Init(this);

            //Set the default notification channel for your app when running Android Oreo
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "ContestParkDefaultChannel";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "ContestParkGeneral";
            }

            FirebasePushNotificationManager.Initialize(this, new NotificationUserCategory[]
        {
            new NotificationUserCategory("message",new List<NotificationUserAction> {
                new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground),
                new NotificationUserAction("Forward","Forward",NotificationActionType.Foreground)
            }),
            new NotificationUserCategory("request",new List<NotificationUserAction> {
                new NotificationUserAction("Accept","Accept",NotificationActionType.Default,"check"),
                new NotificationUserAction("Reject","Reject",NotificationActionType.Default,"cancel")
            })
        }, Debugger.IsAttached);
        }
    }
}
