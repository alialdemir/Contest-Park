using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.ViewModels;
using Rg.Plugins.Popup.Pages;

namespace ContestPark.Mobile.Views
{
    public partial class AcceptDuelInvitationPopupView : PopupPage
    {
        #region Constructor

        public AcceptDuelInvitationPopupView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public InviteModel InviteModel { get; set; }

        #endregion Properties

        #region Overrides

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AcceptDuelInvitationPopupViewModel viewModel = (AcceptDuelInvitationPopupViewModel)BindingContext;
            if (viewModel == null)
                return;

            viewModel.InviteModel = InviteModel;

            //viewModel.InitializeCommand.Execute(null);
            viewModel.IsInitialized = true;
        }

        #endregion Overrides
    }
}
