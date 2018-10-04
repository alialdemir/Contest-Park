using ContestPark.Mobile.Models.PhotoModal;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.PostCardView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePostCard : ContentView
    {
        #region Private

        private bool IsBusy;

        private readonly INavigationService _navigationService;

        #endregion Private

        #region Constructors

        public ImagePostCard(INavigationService navigationService)
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
                    });

                    IsBusy = false;
                }));
            }
        }

        private Command gotoPhotoModalCommand;

        /// <summary>
        /// Fotoğrafı tam ekran olarak gösterir
        /// </summary>
        public Command GotoPhotoModalCommand
        {
            get
            {
                return gotoPhotoModalCommand ?? (gotoPhotoModalCommand = new Command(() =>
                {
                    if (IsBusy)
                        return;

                    IsBusy = true;

                    PostModel model = (PostModel)BindingContext;
                    if (model != null)
                    {
                        _navigationService?.NavigateAsync(nameof(PhotoModalView), new NavigationParameters
                                                    {
                                                         { "userPictureList",  new PhotoModal { PicturePath = model.AlternativePicturePath } }
                                                    }, useModalNavigation: true);
                    }

                    IsBusy = false;
                }));
            }
        }

        #endregion Commands
    }
}