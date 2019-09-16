using ContestPark.Mobile.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileView : ContentPage
    {
        #region Constructor

        public ProfileView()
        {
            InitializeComponent();
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);// Altta tabbar gözükmemesi için ekledim
        }

        #endregion Constructor
    }

    public class MyProfileView : ProfileView
    {
        #region Constructor

        public MyProfileView()
        {
            Shell.SetNavBarIsVisible(this, true);
            Shell.SetTabBarIsVisible(this, true);// Altta tabbar gözükmemesi için ekledim
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ProfileViewModel viewModel = ((ProfileViewModel)BindingContext);

            if (viewModel == null && !viewModel.IsInitialized)
                return;

            NavigationParameters parameters = new NavigationParameters();

            parameters.Add("UserName", viewModel._settingsService.CurrentUser?.UserName);
            parameters.Add("IsVisibleBackArrow", false);

            viewModel.OnNavigatedTo(parameters);
            viewModel.IsInitialized = true;
        }

        #endregion Methods
    }
}
