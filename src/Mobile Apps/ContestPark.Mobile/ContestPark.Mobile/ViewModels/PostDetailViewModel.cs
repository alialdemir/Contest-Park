using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Services.Settings;
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
    public class PostDetailViewModel : ViewModelBase<PostCommentModel>
    {
        #region Private variable

        private readonly IPostService _postService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ISettingsService _settingsService;
        private int postId;

        #endregion Private variable

        #region Properties

        private string _message = string.Empty;
        private PostDetailModel postModel;

        /// <summary>
        /// Yazdığı mesaj
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public PostDetailModel PostModel
        {
            get { return postModel; }
            set
            {
                postModel = value;
                RaisePropertyChanged(() => PostModel);
            }
        }

        #endregion Properties

        #region Constructors

        public PostDetailViewModel(
            INavigationService navigationService,
            IPageDialogService dialogService,
            IPostService postService,
            IAnalyticsService analyticsService,
            ISettingsService settingsService
            ) : base(navigationService, dialogService)
        {
            _postService = postService;
            _analyticsService = analyticsService;
            _settingsService = settingsService;
            NavigationService = navigationService;
            Title = ContestParkResources.Comment;
        }

        #endregion Constructors

        #region Methods

        protected override async Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (parameters.ContainsKey("PostId")) postId = parameters.GetValue<int>("PostId");

            PostModel = await _postService.GetPostByPostIdAsync(postId, ServiceModel);
            if (PostModel != null && PostModel.Comments != null)
            {
                ServiceModel = PostModel.Comments;
            }

            await base.InitializeAsync(parameters);

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
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
        /// Yorum gönder
        /// </summary>
        private async Task ExecuteSendCommentCommand()
        {
            if (IsBusy || string.IsNullOrEmpty(_message))
                return;

            IsBusy = true;

            UserInfoModel currentUser = _settingsService.CurrentUser;

            PostCommentModel postComment = new PostCommentModel
            {
                Date = DateTime.Now,
                Comment = _message.Trim(),
                UserName = currentUser.UserId,
                FullName = currentUser.FullName,
                ProfilePicturePath = currentUser.ProfilePicturePath
            };
            Items.Add(postComment);
            Items.OrderByDescending(x => x.Date);

            bool isSuccess = await _postService.SendCommentAsync(postId, _message);
            if (!isSuccess)
            {
                Items.Remove(postComment);
                await DisplayAlertAsync("", ContestParkResources.GlobalErrorMessage, ContestParkResources.Okay);
            }
            else
            {
                _analyticsService.SendEvent("Post", "Post Yorum", PostModel.Post.PostType.ToString());
            }

            ListViewScrollToBottomCommand?.Execute(Items.Count - 1);

            EditorFocusCommand?.Execute(null);

            Message = string.Empty;

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _gotoProfilePageCommand;
        private ICommand _sendCommentCommand;
        public ICommand EditorFocusCommand { get; set; }

        public ICommand GotoProfilePageCommand =>
                    _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(ExecuteGotoProfilePageCommand));

        /// <summary>
        /// Listview scroll aşağıya çeker
        /// </summary>
        public ICommand ListViewScrollToBottomCommand { get; set; }

        public INavigationService NavigationService { get; }

        /// <summary>
        /// Mesaj gönder command
        /// </summary>
        public ICommand SendCommentCommand
        {
            get { return _sendCommentCommand ?? (_sendCommentCommand = new CommandAsync(ExecuteSendCommentCommand)); }
        }

        #endregion Commands
    }
}
