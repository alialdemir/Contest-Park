using ContestPark.Mobile.Events;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Categories.CategoryDetail;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Analytics;
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
        private readonly IAnalyticsService _analyticsService;
        private short _subCategoryId = 0;
        private readonly PostRefreshEvent _onSleepEvent;

        private SubscriptionToken _subscriptionToken;

        #endregion Private variable

        #region Constructor

        public CategoryDetailViewModel(INavigationService navigationService,
                                       IPopupNavigation popupNavigation,
                                       ICategoryService categoryService,
                                       IPostService postService,
                                       IAnalyticsService analyticsService,
                                       IGameService gameService,
                                       IEventAggregator eventAggregator) : base(navigationService, popupNavigation: popupNavigation)
        {
            NavigationService = navigationService;
            _categoryService = categoryService;
            _postService = postService;
            _analyticsService = analyticsService;
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

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (parameters.ContainsKey("SubCategoryId"))
                _subCategoryId = parameters.GetValue<short>("SubCategoryId");

            if (CategoryDetail.SubCategoryId == 0)
                SubCategoryDetailCommand.Execute(null);

            SubCategoryPostsCommand.Execute(false);

            if (SubscriptionToken == null)
            {
                SubscriptionToken = _onSleepEvent.Subscribe(() => SubCategoryPostsCommand.Execute(true));
            }

            return base.InitializeAsync(parameters);
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private void ExecuteduelOpenPanelCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var selectedSubCategory = new SelectedSubCategoryModel
            {
                SubcategoryId = _subCategoryId,
                SubCategoryName = CategoryDetail.SubCategoryName,
                SubCategoryPicturePath = CategoryDetail.PicturePath,
            };

            NavigateToPopupAsync<DuelBettingPopupView>(new NavigationParameters
            {
                { "SelectedSubCategory", selectedSubCategory }
            });

            _analyticsService.SendEvent("Kategori Detay", "Rakip Bul", CategoryDetail.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Sıralama sayfasına git
        /// </summary>
        private void ExecuteGoToRankingPageCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            NavigateToAsync<RankingView>(new NavigationParameters
            {
                {"SubCategoryId", _subCategoryId },
                {"ListType", RankingViewModel.ListTypes.ScoreRanking },
            });

            _analyticsService.SendEvent("Kategori Detay", "Sıralama", CategoryDetail.SubCategoryName);

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

                _analyticsService.SendEvent("Kategori Detay", "Kategori Takip", CategoryDetail.SubCategoryName);
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
            get { return _duelOpenPanelCommand ?? (_duelOpenPanelCommand = new Command(ExecuteduelOpenPanelCommand)); }
        }

        /// <summary>
        /// Sıralama sayfasına git command
        /// </summary>
        public ICommand GoToRankingPageCommand
        {
            get { return _goToRankingPageCommand ?? (_goToRankingPageCommand = new Command(ExecuteGoToRankingPageCommand)); }
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
            get { return _shareCommand ?? (_shareCommand = new Command(_gameService.SubCategoryShare)); }
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
            get { return _subCategoryFollowProgcessCommand ?? (_subCategoryFollowProgcessCommand = new CommandAsync(ExecuteSubCategoryFollowProgcessCommandAsync)); }
        }

        private ICommand SubCategoryPostsCommand
        {
            get
            {
                return new Command<bool>(async (isForceCache) =>
                {
                    ServiceModel.PageSize = 3;

                    ServiceModel = await _postService.GetPostsBySubCategoryIdAsync(_subCategoryId, ServiceModel, isForceCache: isForceCache);

                    await base.InitializeAsync(new NavigationParameters
                    {
                        { "SubCategoryId", _subCategoryId }
                    });
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
    }
}
