using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.PostCardView
{
    
    public partial class ContestPostCard : ContentView
    {
        #region Private

        private readonly INavigationService _navigationService;
        private bool IsBusy;

        #endregion Private

        #region Constructors

        public ContestPostCard()
        {
            InitializeComponent();
        }

        public ContestPostCard(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        #endregion Constructors

        #region Commands

        private Command<string> gotoProfilePageCommand;

        /// <summary>
        /// Go to ProfilePage load command
        /// </summary>
        public Command<string> GotoProfilePageCommand
        {
            get
            {
                return gotoProfilePageCommand ?? (gotoProfilePageCommand = new Command<string>((userName) =>
                {
                    if (IsBusy || string.IsNullOrEmpty(userName))
                        return;

                    IsBusy = true;

                    _navigationService?.NavigateAsync(nameof(ProfileView), new NavigationParameters
                    {
                         { "UserName", userName }
                    }, useModalNavigation: false);

                    IsBusy = false;
                }));
            }
        }

        #endregion Commands

        #region override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            PostModel postListModel = (PostModel)BindingContext;
            if (postListModel != null)
            {
                lblFounderTrueAnswerCount.TextColor = lblFounderFullName.TextColor = Color.FromHex(postListModel.FounderColor);
                lblCompetitorTrueAnswerCount.TextColor = lblCompetitorFullName.TextColor = Color.FromHex(postListModel.CompetitorColor);
            }
        }

        #endregion override
    }
}
