using Autofac;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using MonkeyCache.SQLite;
using Plugin.Iconize;
using Prism;
using Prism.Autofac;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ContestPark.Mobile
{
    public partial class ContestParkApp : PrismApplication
    {
        #region Constructor

        public ContestParkApp(IPlatformInitializer initializer = null) :base(initializer)
        {
        }

        #endregion Constructor

        #region OnInitialized

        protected override void OnInitialized()
        {
            InitializeComponent();

            Iconize.With(new Plugin.Iconize.Fonts.FontAwesomeBrandsModule())
                   .With(new Plugin.Iconize.Fonts.FontAwesomeRegularModule())
                   .With(new Plugin.Iconize.Fonts.FontAwesomeSolidModule());

            Barrel.ApplicationId = "ContestPark";

            ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();

            if (string.IsNullOrEmpty(settingsService?.AuthAccessToken))
                NavigationService.NavigateAsync($"{nameof(BaseNavigationPage)}/{nameof(SignInView)}");
            else
                NavigationService.NavigateAsync($"{nameof(MasterDetailView)}/{nameof(BaseNavigationPage)}/{nameof(TabView)}");
        }

        #endregion OnInitialized

        #region Register Types

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            GlobalSetting.Instance.IsMockData = true;

            RegisterTypesConfig.Init(Container, containerRegistry);
        }

        #endregion Register Types

        #region OnResume and OnSleep

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

        #endregion OnResume and OnSleep
    }
}