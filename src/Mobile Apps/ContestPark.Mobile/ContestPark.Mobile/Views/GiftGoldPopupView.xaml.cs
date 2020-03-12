using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GiftGoldPopupView : PopupPage
    {
        #region Constructor

        public GiftGoldPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public decimal GiftGold { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = (GiftGoldPopupViewModel)BindingContext;
            if (viewModel == null)
                return;

            viewModel.GiftGold = GiftGold;
            viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        private void CachedImage_Finish(object sender, FFImageLoading.Forms.CachedImageEvents.FinishEventArgs e)
        {
            if (!IsBusy)
            {
                IsBusy = true;

                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(3000);

                lblTapToOpen.IsVisible = svg.IsVisible = false;

                btnCollect.IsVisible = coins.IsVisible = lblDailyReward.IsVisible = dobleCoinsView.IsVisible = true;

                await lblDailyReward.FadeTo(1, 500, Easing.Linear);

                await dobleCoinsView.FadeTo(1, 500, Easing.Linear);

                await btnCollect.FadeTo(1, 500, Easing.Linear);

                coins.Opacity = 0;
            });
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        #endregion Methods
    }
}
