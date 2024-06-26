﻿using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class CategoriesView : ContentPage
    {
        #region Constructor

        public CategoriesView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CategoriesViewModel viewModel = ((CategoriesViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        #endregion Methods
    }
}
