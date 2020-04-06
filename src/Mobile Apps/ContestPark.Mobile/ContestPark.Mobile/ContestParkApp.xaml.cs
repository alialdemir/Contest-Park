using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MonkeyCache.SQLite;
using Plugin.FirebasePushNotification;
using Plugin.Iconize;
using Prism;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ContestPark.Mobile
{
    public partial class ContestParkApp : PrismApplication
    {
        #region Constructor

        public ContestParkApp(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        #endregion Constructor

        #region OnInitialized

        protected override void OnInitialized()
        {
            try
            {
                Crashes.SentErrorReport += Crashes_SentErrorReport;

                InitializeComponent();

                Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule())
                       .With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule())
                       .With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule());

                Barrel.ApplicationId = "ContestPark";

                ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();

                if (!string.IsNullOrEmpty(settingsService?.AuthAccessToken))
                    NavigationService.NavigateAsync(nameof(AppShell));
                else NavigationService.NavigateAsync($"{nameof(BaseNavigationPage)}/{nameof(PhoneNumberView)}");

                #region Push notification token update to server

                CrossFirebasePushNotification.Current.OnTokenRefresh += (sender, e) =>
                {
                    if (e == null || string.IsNullOrEmpty(e.Token) || string.IsNullOrEmpty(settingsService.AuthAccessToken))
                        return;

                    NotificationService?.UpdatePushTokenAsync(new PushNotificationTokenModel
                    {
                        Token = e.Token
                    });
                };
                CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
                {
                    AnalyticsService?.SendEvent("PushNotification", "Received", "Success");
                };

                #endregion Push notification token update to server
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        /// <summary>
        /// Uygulamada oluşan tüm hataları ga gönderildi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Crashes_SentErrorReport(object sender, SentErrorReportEventArgs e)
        {
            AnalyticsService?.SendEvent("Hata", "Uygulama Hatalari ", e.Report.Exception.Message);
        }

        #endregion OnInitialized

        #region Properties

        private INotificationService _notificationService;

        public INotificationService NotificationService
        {
            get
            {
                if (_notificationService == null)
                {
                    _notificationService = RegisterTypesConfig.Container.Resolve<INotificationService>();
                }

                return _notificationService;
            }
        }

        private IAnalyticsService _analyticsService;

        public IAnalyticsService AnalyticsService
        {
            get
            {
                if (_analyticsService == null)
                {
                    _analyticsService = RegisterTypesConfig.Container.Resolve<IAnalyticsService>();
                }

                return _analyticsService;
            }
        }

        #endregion Properties

        #region Register Types

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if DEBUG
            GlobalSetting.Instance.IsMockData = false;
#endif
            RegisterTypesConfig.Init(Container, containerRegistry);
        }

        #endregion Register Types

        #region OnResume, OnSleep and OnStart

        protected override void OnStart()
        {
            base.OnStart();

            AppCenter.Start(GlobalSetting.AppCenterKey, typeof(Analytics), typeof(Crashes));
        }

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
