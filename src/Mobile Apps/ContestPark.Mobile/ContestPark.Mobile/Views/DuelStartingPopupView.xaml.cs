using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static ContestPark.Mobile.ViewModels.DuelStartingPopupViewModel;

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

        #region Properties

        public decimal Bet { get; set; }
        public string OpponentUserId { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public SelectedSubCategoryModel SelectedSubCategory { get; set; }
        public StandbyModes StandbyMode { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = ((DuelStartingPopupViewModel)BindingContext);

            if (viewModel == null || viewModel.IsInitialized)
                return;

            viewModel.StandbyModeModel.SubCategoryId = viewModel.SelectedSubCategory.SubcategoryId = SelectedSubCategory.SubcategoryId;
            viewModel.StandbyModeModel.Bet = Bet;
            viewModel.StandbyModeModel.BalanceType = BalanceType;
            viewModel.SelectedSubCategory.SubcategoryName = SelectedSubCategory.SubcategoryName;
            viewModel.SelectedSubCategory.SubCategoryPicturePath = SelectedSubCategory.SubCategoryPicturePath;

            viewModel.StandbyMode = StandbyMode;
            viewModel.OpponentUserId = OpponentUserId;

            viewModel.InitializeCommand.Execute(null);
            viewModel.AnimationCommand = new Command(Animate);
            viewModel.IsInitialized = true;
        }

        protected override bool OnBackButtonPressed()
        {
            ((DuelStartingPopupViewModel)BindingContext).DuelCloseCommand.Execute(null);
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
