using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;

namespace ContestPark.Mobile.Views
{
    public partial class RankingView : ContentPage
    {
        #region Constructors

        public RankingView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Overrides

        protected override void OnDisappearing()
        {
            if (BindingContext is RankingViewModel)
                ((RankingViewModel)BindingContext).IsTimerStop = false;

            base.OnDisappearing();
        }

        #endregion Overrides
    }
}
