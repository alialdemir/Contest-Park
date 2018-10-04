using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.PostCardView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContestPostCard : ContentView
    {
        #region Private

        private bool IsBusy;

        private readonly INavigationService _navigationService;

        #endregion Private

        #region Constructors

        public ContestPostCard(INavigationService navigationService)
        {
            _navigationService = navigationService;
            InitializeComponent();
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

                    _navigationService?.NavigateAsync($"{nameof(BaseNavigationPage)}/{nameof(ProfileView)}", new NavigationParameters
                    {
                         { "UserName", userName }
                    });

                    IsBusy = false;
                }));
            }
        }

        private Command<PostModel> gotoDuelResultPageCommand;

        /// <summary>
        /// Go to DuelResultPage load command
        /// </summary>
        public Command<PostModel> GotoDuelResultPageCommand
        {
            get
            {
                return gotoDuelResultPageCommand ?? (gotoDuelResultPageCommand = new Command<PostModel>((model) =>
                {
                    if (IsBusy || model == null)
                        return;

                    IsBusy = true;
                    int duelId = Convert.ToInt32(model?.AlternativeId);
                    // TODO: Burası popup olarak açılması lazım
                    _navigationService?.NavigateAsync(nameof(DuelResultPopupView), new NavigationParameters
                    {
                        { "DuelId", duelId },
                        { "SubCategoryId", model.SubCategoryId },
                        { "IsNavBarShow", false },
                    });

                    IsBusy = false;
                }));
            }
        }

        #endregion Commands
    }
}