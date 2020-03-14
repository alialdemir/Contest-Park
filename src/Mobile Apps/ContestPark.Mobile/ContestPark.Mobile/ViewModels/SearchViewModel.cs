using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SearchViewModel : ViewModelBase<SearchModel>
    {
        #region Private variable

        private readonly ICategoryService _categoryService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IFollowService _followService;
        private readonly IGameService _gameService;
        private short _categoryId = 0;

        #endregion Private variable

        #region Constructors

        public SearchViewModel(ICategoryService categoryServices,
                               IEventAggregator eventAggregator,
                               INavigationService navigationService,
                               IFollowService followService,
                               IGameService gameService) : base(navigationService)
        {
            Title = ContestParkResources.SearchPlayersOrCategories;
            _categoryService = categoryServices;
            _eventAggregator = eventAggregator;
            _followService = followService;
            _gameService = gameService;

            EventSubscribe();
        }

        #endregion Constructors

        #region Properties

        private string _search;

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value.Trim();
                RaisePropertyChanged(() => Search);
            }
        }

        private bool _isSearchFocus;

        /// <summary>
        ///  _categoryId  -1 gelirse takip ettiğim kategoriler demek
        /// </summary>
        public bool IsFollowingCategory
        {
            get { return _categoryId == -1; }
        }

        public bool IsSearchFocus
        {
            get { return _isSearchFocus; }
            set
            {
                _isSearchFocus = value;
                RaisePropertyChanged(() => IsSearchFocus);
            }
        }

        private bool IsActionSheetBusy { get; set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (IsFollowingCategory)
            {
                ServiceModel = await _categoryService.FollowedSubCategoriesAsync("'", ServiceModel);
            }
            else if (_categoryId > 0 || _categoryId == -3 || _categoryId == -2)
            {
                ServiceModel = await _categoryService.SearchAsync("'", _categoryId, ServiceModel);//0 gelirse tüm kategoriler demek 0 dan büyük ise ilgili kategoriyi getirir
            }
            else
            {
                IsSearchFocus = true;
            }

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategori yenile event dinleme
        /// </summary>
        private void EventSubscribe()
        {
            _eventAggregator
                          .GetEvent<SubCategoryRefleshEvent>()
                          .Subscribe(() => RefreshCommand.Execute(null));
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommandAsync(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategori için action sheet açar
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        private void ExecuteSubCategoriesDisplayActionSheetCommand(short subCategoryId)
        {
            if (IsActionSheetBusy)
                return;

            IsActionSheetBusy = true;

            SearchModel selectedModel = Items?.FirstOrDefault(P => P.SubCategoryId == subCategoryId);
            if (selectedModel != null)
            {
                _gameService?.SubCategoriesDisplayActionSheetAsync(new SelectedSubCategoryModel
                {
                    SubcategoryId = selectedModel.SubCategoryId,
                    SubcategoryName = selectedModel.CategoryName,
                    SubCategoryPicturePath = selectedModel.PicturePath
                }, selectedModel.IsCategoryOpen);
            }

            IsActionSheetBusy = false;
        }

        /// <summary>
        /// Kategori giriş sayfasına git
        /// </summary>
        /// <param name="subCategoryId">Alt kategori ıd</param>
        /// <returns></returns>
        private async Task ExecutPushCategoryDetailCommandAsync(short subCategoryId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            SearchModel selectedModel = Items.Where(p => p.SubCategoryId == subCategoryId).First();
            if (selectedModel == null)
            {
                IsBusy = false;
                return;
            }

            await _gameService.PushCategoryDetailViewAsync(
                selectedModel.SubCategoryId,
                selectedModel.IsCategoryOpen,
                selectedModel.CategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Kategori giriş sayfasına git
        /// </summary>
        /// <param name="subCategoryId">Alt kategori ıd</param>
        /// <returns></returns>
        private void ExecutPushEnterPageCommandAsync(short subCategoryId)
        {
            SearchModel selectedModel = Items.Where(p => p.SubCategoryId == subCategoryId).First();
            if (selectedModel != null)
            {
                _gameService?.PushCategoryDetailViewAsync(selectedModel.SubCategoryId,
                                                          selectedModel.IsCategoryOpen,
                                                          selectedModel.CategoryName);
            }
        }

        /// <summary>
        /// Search input changed event
        /// </summary>
        /// <param name="e">Input value</param>
        private async Task ExecutSearchTextCommandAsync(TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Search))
                RefreshCommand.Execute(null);

            if (IsBusy || Search.Length < 3)
                return;

            IsBusy = true;

            if (IsFollowingCategory)
            {
                ServiceModel = await _categoryService.FollowedSubCategoriesAsync(Search, new PagingModel { });
            }
            else
            {
                ServiceModel = await _categoryService.SearchAsync(Search, _categoryId, new PagingModel { });
            }

            Items.Clear();

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Takip et takipten çıkar işlemlerini yapar
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        private async Task ExecuteFollowCommandAsync(string userId)
        {
            if (IsBusy || string.IsNullOrEmpty(userId))
                return;

            IsBusy = true;

            SearchModel followModel = Items.Where(x => x.UserId == userId).First();
            if (followModel == null)
                return;

            Items.Where(x => x.UserId == userId).First().IsFollowing = !followModel.IsFollowing;

            bool isSuccesss = await (followModel.IsFollowing == true ?
                  _followService.FollowUpAsync(userId) :
                  _followService.UnFollowAsync(userId));

            if (!isSuccesss)
            {
                Items.Where(x => x.UserId == userId).First().IsFollowing = !followModel.IsFollowing;

                await DisplayAlertAsync("",
                    ContestParkResources.GlobalErrorMessage,
                    ContestParkResources.Okay);
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _SubCategoriesDisplayActionSheetCommand;
        private ICommand gotoProfilePageCommand;
        private ICommand pushCategoryDetailCommand;
        private ICommand pushEnterPageCommand;

        private ICommand searchTextCommand;

        public ICommand GotoProfilePageCommand
        {
            get
            {
                return gotoProfilePageCommand ?? (gotoProfilePageCommand = new Command<string>(ExecuteGotoProfilePageCommandAsync));
            }
        }

        private ICommand _followCommand;

        public ICommand FollowCommand
        {
            get
            {
                return _followCommand ?? (_followCommand = new Command<string>(async (userId) => await ExecuteFollowCommandAsync(userId)));
            }
        }

        /// <summary>
        /// Push category detailcommand
        /// </summary>
        public ICommand PushCategoryDetailCommand
        {
            get { return pushCategoryDetailCommand ?? (pushCategoryDetailCommand = new Command<short>(async (subCategoryId) => await ExecutPushCategoryDetailCommandAsync(subCategoryId))); }
        }

        /// <summary>
        /// Yarışma kategorileri command
        /// </summary>
        public ICommand PushEnterPageCommand
        {
            get { return pushEnterPageCommand ?? (pushEnterPageCommand = new Command<short>((subCategoryId) => ExecutPushEnterPageCommandAsync(subCategoryId))); }
        }

        /// <summary>
        /// Push category detailcommand
        /// </summary>
        public ICommand SearchTextCommand
        {
            get { return searchTextCommand ?? (searchTextCommand = new Command<TextChangedEventArgs>(async (e) => await ExecutSearchTextCommandAsync(e))); }
        }

        public ICommand ClearSearchCommand
        {
            get => new Command(() => Search = string.Empty);
        }

        public ICommand SubCategoriesDisplayActionSheetCommand => _SubCategoriesDisplayActionSheetCommand ?? (_SubCategoriesDisplayActionSheetCommand = new Command<short>(ExecuteSubCategoriesDisplayActionSheetCommand));

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("CategoryId")) _categoryId = parameters.GetValue<short>("CategoryId");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navigation
    }
}
