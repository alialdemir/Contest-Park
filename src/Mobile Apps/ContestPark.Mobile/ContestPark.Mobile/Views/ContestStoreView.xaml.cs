﻿using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class ContestStoreView : ContentPage
    {
        #region Constructor

        public ContestStoreView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        private ContestStoreViewModel _viewModel;

        public ContestStoreViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ((ContestStoreViewModel)BindingContext);
                }

                return _viewModel;
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            if (ViewModel == null || ViewModel.IsInitialized)
                return;

            ViewModel.InitializeCommand.Execute(null);
            ViewModel.IsInitialized = true;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ViewModel.RemoveOnRewardedVideoAdClosed.Execute(null);

            base.OnDisappearing();
        }

        #endregion Methods
    }
}
