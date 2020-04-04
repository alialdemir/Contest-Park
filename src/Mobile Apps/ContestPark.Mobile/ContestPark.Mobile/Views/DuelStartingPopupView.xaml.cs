using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DuelStartingPopupView : PopupPage
    {
        #region Constructor

        public DuelStartingPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = ((DuelStartingPopupViewModel)BindingContext);
            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.AnimationCommand = new Command(Animate);
        }

        protected override bool OnBackButtonPressed()
        {
            ((DuelStartingPopupViewModel)BindingContext).GotoBackCommand.Execute(null);
            return true;
        }

        private void Animate()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var height = Application.Current.MainPage.Height;

                var founderAnimation = new Animation();
                var topToEnd = new Animation(callback: d => gridFounder.TranslationY = d,
                                               start: -height,
                                               end: 0,
                                               easing: Easing.BounceOut);
                founderAnimation.Add(0, 1, topToEnd);
                founderAnimation.Commit(gridFounder, "Loop", length: 2000);

                var copetitorAnimation = new Animation();
                var endToTop = new Animation(callback: d => gridOpponent.TranslationY = d,
                                                start: height,
                                                  end: 0,
                                               easing: Easing.BounceOut);
                copetitorAnimation.Add(0, 1, endToTop);
                copetitorAnimation.Commit(gridOpponent, "Loop", length: 2000);
            });
        }

        #endregion Methods
    }
}
