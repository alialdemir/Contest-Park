using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using Prism.Events;
using Prism.Ioc;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        #region Constructor

        public AppShell(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            Routing.RegisterRoute("SettingsView", typeof(SettingsView));
            Routing.RegisterRoute("ContestStoreView", typeof(ContestStoreView));
            Routing.RegisterRoute("WinningsView", typeof(WinningsView));
            Routing.RegisterRoute("BalanceCodeView", typeof(BalanceCodeView));

            eventAggregator?// left menu navigation için
                        .GetEvent<TabPageNavigationEvent>()
                        .Subscribe(async (page) => await Current.GoToAsync(page.PageName));

            if (Device.RuntimePlatform == Device.Android)
            {
                Navigated += (e, o) => Current.FlyoutIcon = ImageSource.FromFile("menuicon.png");
                Navigating += (e, o) => Current.FlyoutIcon = ImageSource.FromFile("left_arrow.png");
            }
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Parametreden gelen view adına göre yönlendirme yapar ve sol menuyu kapatır
        /// </summary>
        private void MenuItem_Clicked(object sender, System.EventArgs e)
        {
            if (IsBusy || !(sender is MenuItem))
                return;

            IsBusy = true;

            string name = ((MenuItem)sender).CommandParameter.ToString();

            IAnalyticsService analyticsService = RegisterTypesConfig.Container.Resolve<IAnalyticsService>();

            if (name == "facebook" || name == "twitter" || name == "instagram")
            {
                analyticsService.SendEvent("Sol Menü", "Sosyal Medya", name);

                OpenUri($"https://www.{name}.com/contestpark");
            }
            else if (!string.IsNullOrEmpty(name))
            {
                analyticsService.SendEvent("Sol Menü", "Menü link", name);

                Shell.Current.FlyoutIsPresented = false;

                Current.GoToAsync(name);
            }

            IsBusy = false;
        }

        #endregion Events

        #region Methods

        /// <summary>
        /// gelen linki browser da açar
        /// </summary>
        /// <param name="url">web site link</param>
        private void OpenUri(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Launcher.OpenAsync(url);
            }
        }

        #endregion Methods
    }
}
