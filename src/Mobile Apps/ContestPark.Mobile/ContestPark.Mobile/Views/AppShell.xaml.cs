using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.PageNavigation;
using Prism.Events;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        private readonly IEventAggregator _eventAggregator;

        #region Constructor

        public AppShell(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            Routing.RegisterRoute("SettingsView", typeof(SettingsView));
            Routing.RegisterRoute("ContestStoreView", typeof(ContestStoreView));
            Routing.RegisterRoute("WinningsView", typeof(WinningsView));

            eventAggregator// left menu navigation için
                        .GetEvent<TabPageNavigationEvent>()
                        .Subscribe(async (page) => await Current.GoToAsync(page.PageName));

            Navigated += (s, _) => Shell.Current.FlyoutIcon = ImageSource.FromFile("menuicon.png");

            Navigating += (s, _) => Current.FlyoutIcon = ImageSource.FromFile("left_arrow.png");
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Parametreden gelen view adına göre yönlendirme yapar ve sol menuyu kapatır
        /// </summary>
        private void MenuItem_Clicked(object sender, System.EventArgs e)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string name = ((MenuItem)sender).CommandParameter.ToString();

            if (name == "facebook" || name == "twitter" || name == "instagram")
            {
                OpenUri($"https://www.{name}.com/contestpark");
            }
            else if (!string.IsNullOrEmpty(name))
            {
                Shell.Current.FlyoutIsPresented = false;
                _eventAggregator
                    .GetEvent<TabPageNavigationEvent>()
                    .Publish(new PageNavigation(name));
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
                Device.OpenUri(new Uri(url));
            }
        }

        #endregion Methods
    }
}
