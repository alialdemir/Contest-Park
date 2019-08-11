using ContestPark.Mobile.ViewModels;
using FFImageLoading.Transformations;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DuelResultPopupView : PopupPage
    {
        #region Constructor

        public DuelResultPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public DuelResultPopupViewModel ViewModel
        {
            get { return ((DuelResultPopupViewModel)BindingContext); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel != null && ViewModel.DuelResult != null)
            {
                FounderImage.Transformations.Add(new CircleTransformation(20, ViewModel.DuelResult.FounderColor));
                OpponentImage.Transformations.Add(new CircleTransformation(20, ViewModel.DuelResult.OpponentColor));
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
            ViewModel.ClosePopupCommand.Execute(null);
        }

        #endregion Methods
    }
}
