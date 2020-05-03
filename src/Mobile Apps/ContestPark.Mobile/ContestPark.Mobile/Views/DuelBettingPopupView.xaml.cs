using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    
    public partial class DuelBettingPopupView : PopupPage
    {
        #region Constructor

        public DuelBettingPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

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
            ((DuelBettingPopupViewModel)BindingContext).GotoBackCommand.Execute(null);
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
