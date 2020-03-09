using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Category;
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

        private readonly ICategoryService _categoryService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;
        private readonly IPostService _postService;
        private short _subCategoryId = 0;
        private SubscriptionToken _subscriptionToken;
        private readonly PostRefreshEvent _onSleepEvent;

        #endregion Private variable

        #region Constructor

        public CategoryDetailViewModel(INavigationService navigationService,
                                       IPopupNavigation popupNavigation,
                                       ICategoryService categoryService,
                                       IPostService postService,
                                       IGameService gameService,
                                       IEventAggregator eventAggregator) : base(navigationService, popupNavigation: popupNavigation)
        {
            NavigationService = navigationService;
            _categoryService = categoryService;
            _postService = postService;
            _gameService = gameService;
            _eventAggregator = eventAggregator;
            _onSleepEvent = eventAggregator.GetEvent<PostRefreshEvent>();
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

        protected override Task InitializeAsync()
        {
            if (IsBusy)
                return Task.CompletedTask;

            IsBusy = true;

            if (CategoryDetail.SubCategoryId == 0)
                SubCategoryDetailCommand.Execute(null);

            SubCategoryPostsCommand.Execute(false);

            IsBusy = false;

            if (SubscriptionToken == null)
            {
                SubscriptionToken = _onSleepEvent.Subscribe(() => SubCategoryPostsCommand.Execute(true));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private void ExecuteduelOpenPanelCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PushPopupPageAsync(new DuelBettingPopupView()
            {
                SelectedSubCategory = new SelectedSubCategoryModel
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
        private void ExecuteGoToRankingPageCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PushNavigationPageAsync(nameof(RankingView), new NavigationParameters
            {
                {"SubCategoryId", _subCategoryId },
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

            bool isSuccess = await _categoryService.SubCategoryFollowProgcess(_subCategoryId, !CategoryDetail.IsSubCategoryFollowUpStatus);
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

            if (CategoryDetail.IsSubCategoryFollowUpStatus)
                CategoryDetail.FollowerCount++;
            else
                CategoryDetail.FollowerCount--;
        }

        #endregion Methods

        #region Commands

        private ICommand _duelOpenPanelCommand;

        private ICommand _goToRankingPageCommand;

        private ICommand _shareCommand;

        private ICommand _subCategoryFollowProgcessCommand;

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public ICommand DuelOpenPanelCommand
        {
            get { return _duelOpenPanelCommand ?? (_duelOpenPanelCommand = new Command(() => ExecuteduelOpenPanelCommandAsync())); }
        }

        /// <summary>
        /// Sıralama sayfasına git command
        /// </summary>
        public ICommand GoToRankingPageCommand
        {
            get { return _goToRankingPageCommand ?? (_goToRankingPageCommand = new Command(() => ExecuteGoToRankingPageCommandAsync())); }
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
            get { return _shareCommand ?? (_shareCommand = new Command(() => _gameService?.SubCategoryShare(CategoryDetail.SubCategoryName))); }
        }

        public ICommand SubCategoryDetailCommand
        {
            get
            {
                return new Command(async () =>
                {
                    CategoryDetail = await _categoryService.GetSubCategoryDetail(_subCategoryId);

                    Title = CategoryDetail?.SubCategoryName;
                });
            }
        }

        /// <summary>
        /// Alt kategori takip etme veya takip bırakma command
        /// </summary>
        public ICommand SubCategoryFollowProgcessCommand
        {
            get { return _subCategoryFollowProgcessCommand ?? (_subCategoryFollowProgcessCommand = new Command(async () => await ExecuteSubCategoryFollowProgcessCommandAsync())); }
        }

        private ICommand SubCategoryPostsCommand
        {
            get
            {
                return new Command<bool>(async (isForceCache) =>
                {
                    ServiceModel = await _postService.GetPostsBySubCategoryIdAsync(_subCategoryId, ServiceModel, isForceCache: isForceCache);

                    await base.InitializeAsync();
                });
            }
        }

        public ICommand OnSleepEventCommand
        {
            get
            {
                return new Command(() =>
               {
                   if (SubscriptionToken != null && _onSleepEvent != null)
                   {
                       _onSleepEvent.Unsubscribe(SubscriptionToken);
                   }
               });
            }
        }

        public SubscriptionToken SubscriptionToken { get => _subscriptionToken; set => _subscriptionToken = value; }

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubCategoryId")) _subCategoryId = parameters.GetValue<short>("SubCategoryId");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navigation
    }
}
