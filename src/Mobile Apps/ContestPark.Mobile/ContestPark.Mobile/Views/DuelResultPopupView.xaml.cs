using ContestPark.Mobile.ViewModels;
using FFImageLoading.Transformations;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    
    public partial class DuelResultPopupView : PopupPage
    {
        #region Constructor

        public DuelResultPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DuelResultPopupViewModel viewModel = (DuelResultPopupViewModel)BindingContext;
            if (viewModel != null)
            {
                viewModel.ProfilePictureBorderColorCommand = new Command(() =>
                {
                    if (viewModel.DuelResult == null)
                        return;

                    FounderImage.Transformations.Add(new CircleTransformation(20, viewModel.DuelResult.FounderColor));
                    OpponentImage.Transformations.Add(new CircleTransformation(20, viewModel.DuelResult.OpponentColor));
                });
            }
        }

        #endregion Methods
    }
}
