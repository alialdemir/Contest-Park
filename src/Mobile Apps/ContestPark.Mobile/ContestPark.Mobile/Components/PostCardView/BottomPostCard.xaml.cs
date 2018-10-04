using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Views;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContestPark.Mobile.Components.PostCardView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomPostCard : ContentView
    {
        #region Private

        private bool IsBusy;

        private readonly INavigationService _navigationService;

        #endregion Private

        #region Constructors

        public BottomPostCard(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;
        }

        #endregion Constructors

        #region Properties

        public string LikeText { get; set; }

        public string CommentText { get; set; }

        #endregion Properties

        #region Override

        protected override void OnBindingContextChanged()
        {
            PostModel model = (PostModel)BindingContext;
            if (model != null)
            {
                LikeText = model.LikeCount + " " + ContestParkResources.Like;
                CommentText = model.CommentCount + " " + ContestParkResources.Comment;
            }

            base.OnBindingContextChanged();
        }

        #endregion Override

        #region Methods

        /// <summary>
        /// Postu beğenenleri listeleme sayfasına git
        /// </summary>
        private void ExecuteGoToPostLikesPageCommandAsync()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            PostModel model = (PostModel)BindingContext;

            if (model != null)
            {
                _navigationService?.NavigateAsync(nameof(PostLikesView), new NavigationParameters
                        {
                            { "PostId", model.PostId }
                        });
            }

            IsBusy = false;
        }

        /// <summary>
        /// Beğen beğenemkten vazgeç
        /// </summary>
        private async Task ImageButton_Clicked(object sender, EventArgs e)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            IPostService postService = RegisterTypesConfig.Container.Resolve<IPostService>();

            PostModel model = (PostModel)BindingContext;
            if (model != null && postService != null)
            {
                if (model.IsLike)
                {
                    //un like
                    model.IsLike = !model.IsLike;
                    var isOK = await postService.DisLike(model.PostId);
                    if (!isOK) model.IsLike = !model.IsLike;
                }
                else
                {
                    // like
                    model.IsLike = !model.IsLike;
                    var isOK = await postService.Like(model.PostId);
                    if (!isOK) model.IsLike = !model.IsLike;
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// Post deyay sayfasına git
        /// </summary>
        private void ExecuteGoToPosPostCommentPageCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PostModel model = (PostModel)BindingContext;

            if (model != null)
            {
                _navigationService?.NavigateAsync(nameof(PostView), new NavigationParameters
                            {
                                { "PostId", model.PostId }
                            });
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _goToPostLikesPageCommand;

        public ICommand GoToPostLikesPageCommand
        {
            get { return _goToPostLikesPageCommand ?? (_goToPostLikesPageCommand = new Command(() => ExecuteGoToPostLikesPageCommandAsync())); }
        }

        private ICommand _goToPosPostCommentPageCommand;

        public ICommand GoToPosPostCommentPageCommand
        {
            get { return _goToPosPostCommentPageCommand ?? (_goToPosPostCommentPageCommand = new Command(() => ExecuteGoToPosPostCommentPageCommand())); }
        }

        #endregion Commands
    }
}