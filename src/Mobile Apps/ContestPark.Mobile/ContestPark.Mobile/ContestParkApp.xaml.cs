using Com.OneSignal;
using Com.OneSignal.Abstractions;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;

namespace ContestPark.Mobile
{
    public partial class ContestParkApp : Prism.DryIoc.PrismApplication
    {
        #region Constructor

        public ContestParkApp(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        #endregion Constructor

        #region OnInitialized

        protected override void OnInitialized()
        {
            InitializeComponent();

            AppCenter.Start(GlobalSetting.AppCenterKey, typeof(Crashes), typeof(Analytics));
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
            Crashes.SentErrorReport += Crashes_SentErrorReport;

            PushNotification();

            ISettingsService settingsService = Container.Resolve<ISettingsService>();

            if (!string.IsNullOrEmpty(settingsService?.AuthAccessToken))
                NavigationService.NavigateAsync(nameof(AppShell));
            else
                NavigationService.NavigateAsync($"{nameof(PhoneNumberView)}");

#if !DEBUG
                Container
                       .Resolve<ContestPark.Mobile.Services.LatestVersion.ILatestVersionService>()
                       .IfNotUsingLatestVersionOpenInStore();
#endif
        }

        private void PushNotification()
        {
            //NotificationReceived notificationReceived = delegate (OSNotification notification)
            //{
            //    try
            //    {
            //        System.Console.WriteLine("OneSignal Notification Received:\nMessage: {0}", notification.payload.body);
            //        Dictionary<string, object> additionalData = notification.payload.additionalData;

            //        if (additionalData.Count > 0)
            //            System.Console.WriteLine("additionalData: {0}", additionalData);
            //    }
            //    catch (System.Exception e)
            //    {
            //        System.Console.WriteLine(e.StackTrace);
            //    }
            //};

            //NotificationOpened notificationOpened = delegate (OSNotificationOpenedResult result)
            //{
            //    try
            //    {
            //        System.Console.WriteLine("OneSignal Notification opened:\nMessage: {0}", result.notification.payload.body);
            //        Dictionary<string, object> additionalData = result.notification.payload.additionalData;
            //        if (additionalData.Count > 0)
            //            System.Console.WriteLine("additionalData: {0}", additionalData);

            //        List<Dictionary<string, object>> actionButtons = result.notification.payload.actionButtons;
            //        if (actionButtons.Count > 0)
            //            System.Console.WriteLine("actionButtons: {0}", actionButtons);
            //    }
            //    catch (System.Exception e)
            //    {
            //        System.Console.WriteLine(e.StackTrace);
            //    }
            //};
            //Remove this method to stop OneSignal Debugging
            OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.NONE);

            OneSignal.Current.StartInit(GlobalSetting.OneSignalAppId)
                             .Settings(new Dictionary<string, bool>()
                             {
                                 { IOSSettings.kOSSettingsKeyAutoPrompt, false },
                                 { IOSSettings.kOSSettingsKeyInAppLaunchURL, false }
                             })
                            //  .HandleNotificationReceived(notificationReceived)
                            //  .HandleNotificationOpened(notificationOpened)
                            .EndInit();

            //OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

            // The promptForPushNotificationsWithUserResponse function will show the iOS push notification prompt. We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 7)
            OneSignal.Current.RegisterForPushNotifications();
        }

        /// <summary>
        /// Uygulamada oluşan tüm hataları ga gönderildi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Crashes_SentErrorReport(object sender, SentErrorReportEventArgs e)
        {
            AnalyticsService?.SendEvent("Hata", "Uygulama Hatalari ", e.Report.StackTrace);
        }

        #endregion OnInitialized

        #region Properties

        //protected override IContainerExtension CreateContainerExtension() => PrismContainerExtension.Current;

        private IAnalyticsService _analyticsService;

        public IAnalyticsService AnalyticsService
        {
            get
            {
                if (_analyticsService == null)
                {
                    _analyticsService = Container.Resolve<IAnalyticsService>();
                }

                return _analyticsService;
            }
        }

        #endregion Properties

        #region Register Types

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterTypeForNavigation();
            containerRegistry.RegisterTypeInstance();
        }

        #endregion Register Types

        #region OnResume, OnSleep and OnStart

        protected override void OnResume()
        {
            Container
                    .Resolve<IEventAggregator>()
                    .GetEvent<OnResumeEvent>()
                    .Publish();

            base.OnResume();
        }

        protected override void OnSleep()
        {
            Container
                    .Resolve<IEventAggregator>()
                    .GetEvent<OnSleepEvent>()
                    .Publish();

            base.OnSleep();
        }

        #endregion OnResume, OnSleep and OnStart
    }
}
