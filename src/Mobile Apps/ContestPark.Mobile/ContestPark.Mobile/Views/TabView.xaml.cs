using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.ViewModels;
using Plugin.Iconize;
using Prism.Events;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabView : IconTabbedPage
    {
        #region Constructor

        [System.Obsolete]
        public TabView()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Top);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.FromHex("#66ffc107"));

            Color primary = (Color)ContestParkApp.Current.Resources["Primary"];
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(primary);

            IEventAggregator eventAggregator = RegisterTypesConfig.Container.Resolve<IEventAggregator>();
            eventAggregator?
                        .GetEvent<TabChangeEvent>()
                        .Subscribe((tab) =>// parametreden gelen tab'a gider
                        {
                            byte index = (byte)tab;
                            if (index >= 0 && index < Children.Count)
                            {
                                CurrentPage = Children[index];
                            }
                        });
        }

        #endregion Constructor

        #region Override

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            this.Title = this.CurrentPage.Title;

            NavigationParameters parameters = new NavigationParameters();

            if (this.CurrentPage is ProfileView)
            {
                TabViewModel viewModel = (TabViewModel)BindingContext;
                if (viewModel != null)
                {
                    parameters.Add("UserName", viewModel.CurrentUser?.UserName);
                    parameters.Add("IsVisibleBackArrow", false);
                }
            }

            (this.CurrentPage as INavigatedAware)?.OnNavigatedTo(parameters);
            (this.CurrentPage?.BindingContext as INavigatedAware)?.OnNavigatedTo(parameters);
        }

        #endregion Override
    }
}