﻿using ContestPark.Mobile.Models.PhotoModal;
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

        private readonly INavigationService _navigationService;
        private bool IsBusy;

        #endregion Private

        #region Constructors

        public ImagePostCard(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        #endregion Constructors

        #region Commands

        private Command gotoPhotoModalCommand;
        private Command<string> gotoProfilePageCommand;

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
                                                         { "userPictureList",  new PhotoModal { PicturePath = model.PicturePath } }
                                                    }, useModalNavigation: true);
                    }

                    IsBusy = false;
                }));
            }
        }

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

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            lblDescription.IsVisible = BindingContext != null && !string.IsNullOrEmpty(((PostModel)BindingContext)?.Description);
        }
    }
}
