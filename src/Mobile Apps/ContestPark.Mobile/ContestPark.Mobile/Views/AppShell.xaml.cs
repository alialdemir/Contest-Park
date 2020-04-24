using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels;
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
            Routing.RegisterRoute("MissionsView", typeof(MissionsView));
            Routing.RegisterRoute("WinningsView", typeof(WinningsView));
            Routing.RegisterRoute("BalanceCodeView", typeof(BalanceCodeView));

            eventAggregator?// left menu navigation için
                        .GetEvent<TabPageNavigationEvent>()
                        .Subscribe((page) => Current.GoToAsync(page.PageName));

            if (Device.RuntimePlatform == Device.Android)
            {
                Navigated += (e, o) =>
                {
                    if (Current != null)
                    {
                        Current.FlyoutIcon = ImageSource.FromFile("menuicon.png");
                    }
                };
                Navigating += (e, o) =>
                {
                    if (Current != null)
                    {
                        Current.FlyoutIcon = ImageSource.FromFile("left_arrow.png");
                    }
                };
            }
        }

        #endregion Constructor

        #region Events

        public bool IsLoadedMenuItems { get; set; }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null)
                return;

            ((AppShellViewModel)BindingContext).MenuItems = new Command(() =>
            {
                #region Menu items

                ISettingsService settingsService = RegisterTypesConfig.Container.Resolve<ISettingsService>();

                if (settingsService.CurrentUser.UserId == "34873f81-dfee-4d78-bc17-97d9b9bb-bot" || IsLoadedMenuItems)
                    return;

                MenuItem winningsViewMenu = new MenuItem
                {
                    CommandParameter = $"http://contestpark.com/balancecode.html?q={settingsService.AuthAccessToken}",
                    IconImageSource = ContestParkApp.Current.Resources["MoneyBag"].ToString(),
                    Text = ContestParkResources.ConvertToCash,
                };

                winningsViewMenu.Clicked += MenuItem_Clicked;

                Items.Insert(1, winningsViewMenu);

                MenuItem balanceCodeViewMenu = new MenuItem
                {
                    CommandParameter = nameof(BalanceCodeView),
                    IconImageSource = FontImageSource.FromFile(ContestParkApp.Current.Resources["BalanceCode"].ToString()),
                    Text = ContestParkResources.BalanceCode,
                };

                balanceCodeViewMenu.Clicked += MenuItem_Clicked;

                Items.Insert(2, balanceCodeViewMenu);

                IsLoadedMenuItems = true;

                #endregion Menu items
            });
        }

        /// <summary>
        /// Parametreden gelen view adına göre yönlendirme yapar ve sol menuyu kapatır
        /// </summary>
        private void MenuItem_Clicked(object sender, System.EventArgs e)
        {
            if (IsBusy || !(sender is MenuItem) || Current == null)
                return;

            IsBusy = true;

            string name = ((MenuItem)sender).CommandParameter.ToString();

            IAnalyticsService analyticsService = RegisterTypesConfig.Container.Resolve<IAnalyticsService>();

            if (name.StartsWith("https://") || name.StartsWith("http://"))
            {
                analyticsService.SendEvent("Sol Menü", "Link", name);

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
