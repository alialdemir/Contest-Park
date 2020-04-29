using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Picture;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
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

        private Command gotoPhotoModalCommand;
        private Command _optionsCommand;
        private Command<string> gotoProfilePageCommand;
        private IPageDialogService _pageDialogService;
        private ISettingsService _settingsService;
        private IPostService _postService;

        #endregion Private

        #region Constructors

        public ImagePostCard(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        #endregion Constructors

        #region Commands

        public IPostService PostService
        {
            get
            {
                if (_postService == null)
                    _postService = ContestParkApp.Current.Container.Resolve<IPostService>();

                return _postService;
            }
        }

        public ISettingsService SettingsService
        {
            get
            {
                if (_settingsService == null)
                    _settingsService = ContestParkApp.Current.Container.Resolve<ISettingsService>();

                return _settingsService;
            }
        }

        public IPageDialogService PageDialogService
        {
            get
            {
                if (_pageDialogService == null)
                    _pageDialogService = ContestParkApp.Current.Container.Resolve<IPageDialogService>();

                return _pageDialogService;
            }
        }

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

                    PostModel postModel = (PostModel)BindingContext;
                    INavigationService navigationService = ContestParkApp.Current.Container.Resolve<INavigationService>();

                    if (postModel != null && navigationService != null)
                    {
                        navigationService.NavigateAsync(nameof(PhotoModalView), new NavigationParameters
                        {
                            {
                                "Pictures", new List<PictureModel>
                            {
                                new PictureModel { PicturePath = postModel.PicturePath }
                            }
                            }
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

        public Command OptionsCommand
        {
            get
            {
                return _optionsCommand ?? (_optionsCommand = new Command(async () =>
                {
                    PostModel post = (PostModel)BindingContext;
                    if (IsBusy || post == null)
                        return;

                    IsBusy = true;

                    bool isSuccess = true;

                    List<string> buttons = new List<string>();

                    if (SettingsService.CurrentUser.UserName == post.OwnerUserName)// TODO: user id göre kontrol etsek daha iyi olur
                    {
                        buttons.Add(ContestParkResources.Archive);
                        buttons.Add(post.IsCommentOpen ? ContestParkResources.TurnOffComment : ContestParkResources.TurnOnComment);
                    }

                    buttons.Add(ContestParkResources.Report);

                    string selectedItem = await PageDialogService?.DisplayActionSheetAsync(string.Empty,
                                                                                           ContestParkResources.Cancel,
                                                                                           string.Empty,
                                                                                           buttons.ToArray());
                    if (selectedItem == ContestParkResources.Archive)
                    {
                        isSuccess = await PostService.ArchiveAsync(post.PostId);
                        if (isSuccess)
                        {
                            IEventAggregator eventAggregator = ContestParkApp.Current.Container.Resolve<IEventAggregator>();
                            eventAggregator
                                .GetEvent<PostRefreshEvent>()
                                .Publish();
                        }
                    }
                    else if (selectedItem == ContestParkResources.TurnOffComment || selectedItem == ContestParkResources.TurnOnComment)
                    {
                        isSuccess = await PostService.TurnOffToggleCommentAsync(post.PostId);
                        if (isSuccess)
                            post.IsCommentOpen = !post.IsCommentOpen;
                    }
                    //else if (selectedItem == ContestParkResources.Report)
                    //{
                    //}

                    if (!isSuccess)
                    {
                        await PageDialogService.DisplayAlertAsync(string.Empty,
                                                                  ContestParkResources.GlobalErrorMessage,
                                                                  ContestParkResources.Okay);
                    }

                    IsBusy = false;
                }));
            }
        }

        #endregion Commands

        #region Overrides

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            lblDescription.IsVisible = BindingContext != null && !string.IsNullOrEmpty(((PostModel)BindingContext)?.Description);
        }

        #endregion Overrides
    }
}
