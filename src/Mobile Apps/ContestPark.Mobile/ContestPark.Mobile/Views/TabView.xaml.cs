using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabView : TabbedPage
    {
        #region Constructor

        public TabView()
        {
            InitializeComponent();
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