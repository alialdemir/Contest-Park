using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels;
using Prism.Events;
using Prism.Ioc;
using Prism.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    
    public partial class AppShell : Shell
    {
        #region Constructor

        public AppShell(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            Routing.RegisterRoute("SettingsView", typeof(SettingsView));
            Routing.RegisterRoute("ContestStoreView", typeof(ContestStoreView));
            Routing.RegisterRoute("MissionsView", typeof(MissionsView));
            Routing.RegisterRoute("WinningsView", typeof(WinningsView));
            Routing.RegisterRoute("BalanceCodeView", typeof(BalanceCodeView));

            eventAggregator?// left menu navigation için
                        .GetEvent<TabPageNavigationEvent>()
                        .Subscribe((page) => Current.GoToAsync(page.PageName));
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Parametreden gelen view adına göre yönlendirme yapar ve sol menuyu kapatır
        /// </summary>
        private void MenuItem_Clicked(object sender, System.EventArgs e)
        {
            if (IsBusy || !(sender is MenuItem) || Current == null)
                return;

            IsBusy = true;

            string name = ((MenuItem)sender).CommandParameter.ToString();

            ISettingsService settingsService = ContestParkApp.Current.Container.Resolve<ISettingsService>();

            if (settingsService.CurrentUser.UserId == "34873f81-dfee-4d78-bc17-97d9b9bb-bot"
                && (name.StartsWith("http://contestpark.com/balancecode.html") || name.StartsWith("BalanceCodeView")))
            {
                ContestParkApp
                      .Current
                      .Container
                      .Resolve<IPageDialogService>()
                      .DisplayAlertAsync(string.Empty,
                                         ContestParkResources.ComingSoon,
                                         ContestParkResources.Okay);

                IsBusy = false;

                return;
            }

            IAnalyticsService analyticsService = ContestParkApp.Current.Container.Resolve<IAnalyticsService>();

            if (name.StartsWith("https://") || name.StartsWith("http://"))
            {
                analyticsService.SendEvent("Sol Menü", "Link", name);

                if (name.StartsWith("http://contestpark.com/balancecode.html"))
                {
                    name = string.Format(name, settingsService.AuthAccessToken);
                }

                OpenUri(name);

                Shell.Current.FlyoutIsPresented = false;
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
