﻿using ContestPark.Mobile.ViewModels;
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
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IconOverride = ImageSource.FromFile("menuicon.png") });
            Shell.SetNavBarIsVisible(this, true);
            Shell.SetTabBarIsVisible(this, true);// Altta tabbar gözükmemesi için ekledim
        }

        #endregion Constructor

        #region Override

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ProfileViewModel viewModel = ((ProfileViewModel)BindingContext);
            if (viewModel == null || viewModel.IsInitialized)
                return;

            NavigationParameters parameters = new NavigationParameters();

            parameters.Add("UserName", viewModel._settingsService?.CurrentUser?.UserName);
            parameters.Add("IsVisibleBackArrow", false);

            await viewModel.InitializeAsync(parameters);
            viewModel.IsInitialized = true;
        }

        #endregion Override
    }
}
