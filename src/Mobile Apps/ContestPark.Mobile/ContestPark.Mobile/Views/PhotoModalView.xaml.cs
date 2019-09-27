using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.ViewModels;
using MvvmHelpers;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoModalView : PopupPage
    {
        #region Constructor

        public PhotoModalView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public ObservableRangeCollection<PictureModel> Pictures { get; set; }
        public int SelectedIndex { get; set; } = 0;

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Pictures.Count != 0)
            {
                listPictures.ItemsSource = Pictures;
                listPictures.SelectedIndex = SelectedIndex;
            }
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
            ((PhotoModalViewModel)BindingContext).ClosePopupCommand.Execute(null);
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
