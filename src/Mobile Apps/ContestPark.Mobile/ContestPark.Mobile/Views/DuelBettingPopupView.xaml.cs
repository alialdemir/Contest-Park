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

        public short SubcategoryId { get; set; }

        public string SubcategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DuelBettingPopupViewModel viewModel = ((DuelBettingPopupViewModel)BindingContext);

            if (viewModel == null && !viewModel.IsInitialized)
                return;

            viewModel.SelectedSubCategory.SubcategoryId = SubcategoryId;
            viewModel.SelectedSubCategory.SubcategoryName = SubcategoryName;
            viewModel.SelectedSubCategory.SubCategoryPicturePath = SubCategoryPicturePath;

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