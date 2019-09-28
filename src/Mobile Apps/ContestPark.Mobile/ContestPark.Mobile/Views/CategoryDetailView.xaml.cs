﻿using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryDetailView : ContentPage
    {
        #region Constructor

        public CategoryDetailView()
        {
            InitializeComponent();
            Shell.SetTabBarIsVisible(this, false);// Altta tabbar gözükmemesi için ekledim
        }

        #endregion Constructor

        #region Overrides

        protected override void OnDisappearing()
        {
            ((CategoryDetailViewModel)BindingContext).OnSleepEventCommand?.Execute(null);

            base.OnDisappearing();
        }

        #endregion Overrides
    }
}
