using Plugin.Iconize;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabView : IconTabbedPage
    {
        #region Constructor

        public TabView()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Top);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.FromHex("#66ffc107"));

            Color primary = (Color)ContestParkApp.Current.Resources["Primary"];
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarSelectedItemColor(primary);
        }

        #endregion Constructor

        #region Override

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            this.Title = this.CurrentPage.Title;

            // NavigationParameters parameters = new NavigationParameters();
            //if (this.CurrentPage is ProfilePage)
            //{
            //    TabViewModel viewModel = (TabViewModel)BindingContext;
            //    if (viewModel != null)
            //    {
            //        parameters.Add("UserName", viewModel.UserDataModule.UserModel.UserName);
            //        parameters.Add("IsVisibleBackArrow", false);
            //    }
            //}

            //    (this.CurrentPage as INavigatedAware)?.OnNavigatedTo(parameters);
            //(this.CurrentPage?.BindingContext as INavigatedAware)?.OnNavigatedTo(parameters);
        }

        #endregion Override
    }
}