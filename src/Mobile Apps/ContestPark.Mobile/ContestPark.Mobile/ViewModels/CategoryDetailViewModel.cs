using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.CategoryFollow;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CategoryDetailViewModel : ViewModelBase<PostModel>
    {
        #region Private variable

        private readonly ICategoryFollowService _categoryFollowService;
        private readonly ICategoryService _categoryService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;
        private readonly IPostService _postService;
        private short _subCategoryId = 0;

        #endregion Private variable

        #region Constructor

        public CategoryDetailViewModel(INavigationService navigationService,
                                       IPopupNavigation popupNavigation,
                                       ICategoryService categoryService,
                                       ICategoryFollowService categoryFollowService,
                                       IPostService postService,
                                       IGameService gameService,
                                       IEventAggregator eventAggregator) : base(navigationService, popupNavigation: popupNavigation)
        {
            NavigationService = navigationService;
            _categoryService = categoryService;
            _categoryFollowService = categoryFollowService;
            _postService = postService;
            _gameService = gameService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        private CategoryDetailModel _categoryDetail;

        /// <summary>
        /// Kategori bilgileri
        /// </summary>
        public CategoryDetailModel CategoryDetail
        {
            get => _categoryDetail ?? (_categoryDetail = new CategoryDetailModel());
            set
            {
                _categoryDetail = value;
                RaisePropertyChanged(() => CategoryDetail);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            SubCategoryDetailCommand.Execute(null);

            SubCategoryPostsCommand.Execute(null);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private async Task ExecuteduelOpenPanelCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SelectedSubCategory = new Models.Duel.SelectedSubCategoryModel
                {
                    SubcategoryId = _subCategoryId,
                    SubcategoryName = CategoryDetail.SubCategoryName,
                    SubCategoryPicturePath = CategoryDetail.PicturePath,
                }
            });

            IsBusy = false;
        }

        /// <summary>
        /// Sıralama sayfasına git
        /// </summary>
        private async Task ExecuteGoToRankingPageCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await PushNavigationPageAsync(nameof(RankingView), new NavigationParameters
            {
                {"SubCategoryId", _subCategoryId },
                {"SubCategoryName", CategoryDetail.SubCategoryName },
                {"ListType", RankingViewModel.ListTypes.ScoreRanking },
            });

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategori takip et takibi bırak methodu takip ediyorsa takibi bırakır takip etmiyorsa takip eder
        /// </summary>
        private async Task ExecuteSubCategoryFollowProgcessCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            FollowCountChange();

            bool isSuccess = await _categoryFollowService.SubCategoryFollowProgcess(_subCategoryId, CategoryDetail.IsSubCategoryFollowUpStatus);
            if (isSuccess)
            {
                _eventAggregator
                            .GetEvent<SubCategoryRefleshEvent>()
                            .Publish();
            }
            else FollowCountChange();// eğer işlem başarısız ise takibi geri aldık

            IsBusy = false;
        }

        /// <summary>
        /// Kategori takipçi sayısını artırıp azaltır ve takip etme durumunu değiştirir
        /// </summary>
        private void FollowCountChange()
        {
            CategoryDetail.IsSubCategoryFollowUpStatus = !CategoryDetail.IsSubCategoryFollowUpStatus;

            if (CategoryDetail.IsSubCategoryFollowUpStatus) CategoryDetail.CategoryFollowersCount++;
            else CategoryDetail.CategoryFollowersCount--;
        }

        #endregion Methods

        #region Commands

        private ICommand duelOpenPanelCommand;

        private ICommand goToRankingPageCommand;

        private ICommand shareCommand;

        private ICommand subCategoryFollowProgcessCommand;

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public ICommand DuelOpenPanelCommand
        {
            get { return duelOpenPanelCommand ?? (duelOpenPanelCommand = new Command(async () => await ExecuteduelOpenPanelCommandAsync())); }
        }

        /// <summary>
        /// Sıralama sayfasına git command
        /// </summary>
        public ICommand GoToRankingPageCommand
        {
            get { return goToRankingPageCommand ?? (goToRankingPageCommand = new Command(async () => await ExecuteGoToRankingPageCommandAsync())); }
        }

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public INavigationService NavigationService { get; }

        /// <summary>
        /// Sosyal ağda paylaş command
        /// </summary>
        public ICommand ShareCommand
        {
            get { return shareCommand ?? (shareCommand = new Command(() => _gameService?.SubCategoryShare(CategoryDetail.SubCategoryName))); }
        }

        public ICommand SubCategoryDetailCommand
        {
            get
            {
                return new Command(async () => CategoryDetail = await _categoryService.GetSubCategoryDetail(_subCategoryId));
            }
        }

        /// <summary>
        /// Alt kategori takip etme veya takip bırakma command
        /// </summary>
        public ICommand SubCategoryFollowProgcessCommand
        {
            get { return subCategoryFollowProgcessCommand ?? (subCategoryFollowProgcessCommand = new Command(async () => await ExecuteSubCategoryFollowProgcessCommandAsync())); }
        }

        private ICommand SubCategoryPostsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    ServiceModel = await _postService.GetPostsBySubCategoryIdAsync(_subCategoryId, ServiceModel);
                });
            }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubCategoryId")) _subCategoryId = parameters.GetValue<short>("SubCategoryId");
            if (parameters.ContainsKey("SubCategoryId")) _subCategoryId = parameters.GetValue<short>("SubCategoryId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}
