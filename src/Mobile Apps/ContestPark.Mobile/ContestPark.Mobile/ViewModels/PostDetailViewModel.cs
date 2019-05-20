using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.User;
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
        private readonly ISettingsService _settingsService;
        private string postId;

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
            ISettingsService settingsService
            ) : base(navigationService, dialogService)
        {
            _postService = postService;
            _settingsService = settingsService;
            NavigationService = navigationService;
            Title = ContestParkResources.Comment;
        }

        #endregion Constructors

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PostModel = await _postService.GetPostByPostIdAsync(postId);
            if (PostModel != null && PostModel.Likes != null)
            {
                ServiceModel = PostModel.Comments;
            }

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            await PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
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

            ListViewScrollToBottomCommand?.Execute(Items.Count - 1);

            EditorFocusCommand?.Execute(null);

            Message = string.Empty;

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _gotoProfilePageCommand;
        private ICommand sendCommentCommand;
        public ICommand EditorFocusCommand { get; set; }

        public ICommand GotoProfilePageCommand =>
                    _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)));

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
            get { return sendCommentCommand ?? (sendCommentCommand = new Command(async () => await ExecuteSendCommentCommand())); }
        }

        #endregion Commands

        #region Navigations

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("PostId")) postId = parameters.GetValue<string>("PostId");

            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            //base.OnNavigatingTo(parameters);
        }

        #endregion Navigations
    }
}