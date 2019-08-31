using ContestPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
