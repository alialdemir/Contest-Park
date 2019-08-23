﻿using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DuelBettingPopupView : PopupPage
    {
        #region Constructor

        public DuelBettingPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string OpponentUserId { get; set; }

        public SelectedSubCategoryModel SelectedSubCategory { get; set; }


        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DuelBettingPopupViewModel viewModel = ((DuelBettingPopupViewModel)BindingContext);

            if (viewModel == null && !viewModel.IsInitialized)
                return;

            viewModel.SelectedSubCategory = SelectedSubCategory;
            viewModel.OpponentUserId = OpponentUserId;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        protected override bool OnBackButtonPressed()
        {
            CloseAllPopup();
            return true;
        }

        /// <summary>
        /// Popupun
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();
            return false;
        }

        /// <summary>
        /// Popup kapat
        /// </summary>
        private void CloseAllPopup()
        {
            ((DuelBettingPopupViewModel)BindingContext).ClosePopupCommand.Execute(null);
        }

        /// <summary>
        /// İptale basınca popup kapat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        #endregion Methods

    }
}