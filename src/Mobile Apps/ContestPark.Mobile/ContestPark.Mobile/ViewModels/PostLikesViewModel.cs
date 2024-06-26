﻿using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Post.PostLikes;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PostLikesViewModel : ViewModelBase<PostLikeModel>
    {
        #region Private variables

        private readonly IFollowService _followService;
        private readonly IPostService _postService;
        private int postId;

        #endregion Private variables

        #region Constructor

        public PostLikesViewModel(
                INavigationService navigationService,
                IPageDialogService dialogService,
                IFollowService followService,
                IPostService postService
            ) : base(navigationService, dialogService)
        {
            _followService = followService;
            _postService = postService;
            Title = ContestParkResources.PostLikes;
        }

        #endregion Constructor

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("PostId")) postId = parameters.GetValue<int>("PostId");

            PostLikesCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Kullanıcı takip et takipten çıkar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        private async Task ExecuteFollowCommand(string userId)
        {
            if (IsBusy || string.IsNullOrEmpty(userId))
                return;

            IsBusy = true;

            PostLikeModel postLikeModel = Items.Where(x => x.UserId == userId).First();
            if (postLikeModel == null)
                return;

            Items.Where(x => x.UserId == userId).First().IsFollowing = !postLikeModel.IsFollowing;

            bool isSuccesss = await (postLikeModel.IsFollowing == true ?
                  _followService.UnFollowAsync(userId) :
                  _followService.FollowUpAsync(userId));

            if (!isSuccesss)
            {
                Items.Where(x => x.UserId == userId).First().IsFollowing = !postLikeModel.IsFollowing;

                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Profil sayfasına yönlendirir
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            NavigateToAsync<ProfileView>(new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Postu beğenenlerin listesini verir
        /// </summary>
        private async Task ExecutePostLikesCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _postService.PostLikesAsync(postId, ServiceModel);

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand PostLikesCommand => new CommandAsync(ExecutePostLikesCommandAsync);

        public ICommand _followCommand { get; set; }
        public ICommand _gotoProfilePageCommand { get; set; }

        public ICommand FollowCommand
        {
            get { return _followCommand ?? (_followCommand = new CommandAsync<string>(ExecuteFollowCommand)); }
        }

        public ICommand GotoProfilePageCommand
        {
            get { return _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(ExecuteGotoProfilePageCommand)); }
        }

        #endregion Commands
    }
}
