using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class CategoryDetailView : ContentPage
    {
        #region Constructor

        public CategoryDetailView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Overrides

        protected override void OnAppearing()
        {
            base.OnAppearing();

            pnkStats.TranslateTo(0, -50, 1500, Easing.SinInOut);
        }

        protected override void OnDisappearing()
        {
            if (BindingContext != null)
                ((CategoryDetailViewModel)BindingContext).OnSleepEventCommand?.Execute(null);

            base.OnDisappearing();
        }

        #endregion Overrides
    }
}
