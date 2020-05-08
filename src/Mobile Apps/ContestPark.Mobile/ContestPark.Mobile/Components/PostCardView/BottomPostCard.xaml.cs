﻿using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Components.PostCardView
{
    public partial class BottomPostCard : ContentView
    {
        #region Private varaibles

        private readonly INavigationService _navigationService;
        private bool IsBusy;

        #endregion Private varaibles

        #region Constructor

        public BottomPostCard(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Post deyay sayfasına git
        /// </summary>
        private void ExecuteGoToPosPostCommentViewCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PostModel postModel = (PostModel)BindingContext;
            if (postModel != null)
            {
                _navigationService?.NavigateAsync(nameof(PostDetailView), new NavigationParameters
            {
                { "PostId", postModel.PostId }
            }, useModalNavigation: false);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Postu beğenenleri listeleme sayfasına git
        /// </summary>
        private void ExecuteGoToPostLikesViewCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PostModel postModel = (PostModel)BindingContext;
            if (postModel != null && postModel.LikeCount != 0)
            {
                _navigationService?.NavigateAsync(nameof(PostLikesView), new NavigationParameters
            {
                { "PostId", postModel.PostId }
            }, useModalNavigation: false);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Postu beğenmiyorsa beğen beğenmişse beğenmekten vazgeç
        /// </summary>
        private async Task ExecuteLikeProcessCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            IPostService postService = ContestParkApp.Current.Container.Resolve<IPostService>();
            PostModel postModel = (PostModel)BindingContext;

            if (postService == null || postModel == null)
            {
                IsBusy = false;

                return;
            }
            if (postModel.IsLike)
                postModel.LikeCount -= 1;
            else
                postModel.LikeCount += 1;

            postModel.IsLike = !postModel.IsLike;

            bool isSuccess = await (postModel.IsLike ?
                postService.LikeAsync(postModel.PostId) :
                postService.DisLikeAsync(postModel.PostId));

            if (!isSuccess)
            {
                if (postModel.IsLike)
                    postModel.LikeCount -= 1;
                else
                    postModel.LikeCount += 1;

                postModel.IsLike = !postModel.IsLike;
            }
            else
            {
                ContestParkApp
                    .Current
                    .Container
                    .Resolve<IEventAggregator>()
                    .GetEvent<PostRefreshEvent>()
                    .Publish();

                ContestParkApp
                         .Current
                         .Container
                         .Resolve<IAnalyticsService>()
                         .SendEvent("Post",
                                    postModel.IsLike ? "Post Beğenmekten Vazgeç" : "Post Beğen",
                                    postModel.PostType.ToString());
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _goToPosPostCommentViewCommand;
        private ICommand _goToPostLikesViewCommand;

        private ICommand _likeProcessCommand;

        public ICommand GoToPosPostCommentViewCommand
        {
            get { return _goToPosPostCommentViewCommand ?? (_goToPosPostCommentViewCommand = new Command(() => ExecuteGoToPosPostCommentViewCommand())); }
        }

        public ICommand GoToPostLikesViewCommand
        {
            get { return _goToPostLikesViewCommand ?? (_goToPostLikesViewCommand = new Command(() => ExecuteGoToPostLikesViewCommandAsync())); }
        }

        public ICommand LikeProcessCommand
        {
            get { return _likeProcessCommand ?? (_likeProcessCommand = new Command(async () => await ExecuteLikeProcessCommandAsync())); }
        }

        #endregion Commands
    }
}
