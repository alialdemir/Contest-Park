using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MonkeyCache.SQLite;
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
            _analyticsService?.SendEvent("Hata", "Uygulama Hatalari ", e.Report.Exception.Message);
        }

        #endregion OnInitialized

        #region Properties

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
