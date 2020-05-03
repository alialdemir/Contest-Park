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

        protected override void OnDisappearing()
        {
            if (BindingContext != null)
                ((CategoryDetailViewModel)BindingContext).OnSleepEventCommand?.Execute(null);

            base.OnDisappearing();
        }

        #endregion Overrides
    }
}
