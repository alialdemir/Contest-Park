using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.LatestVersion;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MonkeyCache.SQLite;
using Prism;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

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

        protected override async void OnInitialized()
        {
            try
            {
                Crashes.SentErrorReport += Crashes_SentErrorReport;

                InitializeComponent();

                Barrel.ApplicationId = "ContestPark";

                await RegisterTypesConfig
                                   .Container
                                   .Resolve<ILatestVersionService>()
                                   .IfNotUsingLatestVersionOpenInStore();

                ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();

                if (!string.IsNullOrEmpty(settingsService?.AuthAccessToken))
                    await NavigationService.NavigateAsync(nameof(AppShell));
                else await NavigationService.NavigateAsync($"{nameof(BaseNavigationPage)}/{nameof(PhoneNumberView)}");
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

        protected override IContainerExtension CreateContainerExtension() => PrismContainerExtension.Current;

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
