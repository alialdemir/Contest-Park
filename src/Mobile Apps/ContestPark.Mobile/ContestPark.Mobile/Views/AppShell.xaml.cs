using ContestPark.Mobile.Events;
using Prism.Events;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class AppShell : Shell
    {
        #region Constructor

        public AppShell(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            Routing.RegisterRoute("SettingsView", typeof(SettingsView));
            Routing.RegisterRoute("NoTabContestStoreView ", typeof(NoTabContestStoreView));
            Routing.RegisterRoute("ContestStoreView", typeof(ContestStoreView));
            Routing.RegisterRoute("MissionsView", typeof(MissionsView));
            Routing.RegisterRoute("WinningsView", typeof(WinningsView));
            Routing.RegisterRoute("IbanNoView", typeof(IbanNoView));
            Routing.RegisterRoute("BalanceCodeView", typeof(BalanceCodeView));
            Routing.RegisterRoute("BrowserView", typeof(BrowserView));

            eventAggregator?// left menu navigation için
                        .GetEvent<TabPageNavigationEvent>()
                        .Subscribe((page) => Current.GoToAsync(page.PageName));

            eventAggregator?// Seçilen taba yönlendirir
                        .GetEvent<TabChangeEvent>()
                        .Subscribe((selectedTab) =>
                        {
                            Current.FlyoutIsPresented = false;

                            CurrentItem.CurrentItem = CurrentItem.Items[(int)selectedTab];
                        });
        }

        #endregion Constructor
    }
}
