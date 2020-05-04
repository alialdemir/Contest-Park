using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Events;
using Prism.Ioc;

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
            try
            {
                InitializeComponent();

                AppCenter.Start(GlobalSetting.AppCenterKey, typeof(Crashes));
                Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
                Crashes.SentErrorReport += Crashes_SentErrorReport;

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
