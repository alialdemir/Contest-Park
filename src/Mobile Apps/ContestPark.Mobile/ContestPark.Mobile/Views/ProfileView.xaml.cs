using ContestPark.Mobile.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
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

        #region Override

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ProfileViewModel viewModel = ((ProfileViewModel)BindingContext);
            if (viewModel == null || viewModel.IsInitialized)
                return;

            NavigationParameters parameters = new NavigationParameters
            {
                { "UserName", viewModel._settingsService?.CurrentUser?.UserName },
                { "IsVisibleBackArrow", false }
            };

            viewModel.Initialize(parameters);
            viewModel.IsInitialized = true;
        }

        #endregion Override
    }
}
