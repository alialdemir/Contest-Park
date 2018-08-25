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
            var viewModel = ((DuelBettingPopupViewModel)BindingContext);

            if (viewModel == null)
                return;

            viewModel.SubcategoryId = SubcategoryId;
            viewModel.SubcategoryName = SubcategoryName;
            viewModel.SubCategoryPicturePath = SubCategoryPicturePath;

            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
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

        /// <summary>
        /// Popupun
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            CloseAllPopup();
            return true;
        }

        /// <summary>
        /// Popup kapat
        /// </summary>
        private void CloseAllPopup()
        {
            ((DuelBettingPopupViewModel)BindingContext).ClosePopupCommand.Execute(null);
        }

        #endregion Methods
    }
}