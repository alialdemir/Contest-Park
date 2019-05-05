using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Post.PostLikes;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class PostLikesViewModel : ViewModelBase<PostLikeModel>
    {
        #region Private variables

        private readonly IPostService _postService;
        private string postId;

        #endregion Private variables

        #region Constructor

        public PostLikesViewModel(
                INavigationService navigationService,
                IPostService postService
            ) : base(navigationService)
        {
            _postService = postService;
            Title = ContestParkResources.PostLikes;
        }

        #endregion Constructor

        #region Methods

        protected override async Task InitializeAsync()
        {
            ServiceModel = await _postService.PostLikes(postId, ServiceModel);

            await base.InitializeAsync();
        }

        /// <summary>
        /// Profil sayfasına yönlendirir
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
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

        #endregion Methods

        #region Commands

        public ICommand _gotoProfilePageCommand { get; set; }

        public ICommand GotoProfilePageCommand
        {
            get { return _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName))); }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("PostId")) postId = parameters.GetValue<string>("PostId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}