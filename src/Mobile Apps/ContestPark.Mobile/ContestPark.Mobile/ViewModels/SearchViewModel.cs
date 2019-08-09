using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.CategoryFollow;
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

        private readonly ICategoryFollowService _categoryFollowService;
        private readonly ICategoryService _categoryService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;
        private short _categoryId = 0;

        #endregion Private variable

        #region Constructors

        public SearchViewModel(ICategoryFollowService categoryFollowService,
                                       ICategoryService categoryServices,
                                       IEventAggregator eventAggregator,
                                       IGameService gameService)
        {
            Title = ContestParkResources.SearchPlayersOrCategories;
            _categoryFollowService = categoryFollowService;
            _categoryService = categoryServices;
            _eventAggregator = eventAggregator;
            _gameService = gameService;

            EventSubscribe();
        }

        #endregion Constructors

        #region Properties

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
                ServiceModel = await _categoryFollowService.FollowedSubCategoriesAsync("'", ServiceModel);
            }
            else if (_categoryId > 0)
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
        private async Task ExecuteGotoProfilePageCommandAsync(string userName)
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
        /// Alt kategori için action sheet açar
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        private async Task ExecuteSubCategoriesDisplayActionSheetCommand(short subCategoryId)
        {
            if (IsActionSheetBusy)
                return;

            IsActionSheetBusy = true;

            SearchModel selectedModel = Items?.FirstOrDefault(P => P.SubCategoryId == subCategoryId);
            if (selectedModel != null)
            {
                await _gameService?.SubCategoriesDisplayActionSheetAsync(selectedModel.SubCategoryId, selectedModel.CategoryName, selectedModel.IsCategoryOpen);
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

            await _gameService.PushCategoryDetailViewAsync(selectedModel.SubCategoryId, selectedModel.IsCategoryOpen);

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
                                                          selectedModel.IsCategoryOpen);
            }
        }

        /// <summary>
        /// Search input changed event
        /// </summary>
        /// <param name="e">Input value</param>
        private async Task ExecutSearchTextCommandAsync(TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                RefreshCommand.Execute(null);

            if (IsBusy || e.NewTextValue.Length < 3)
                return;

            IsBusy = true;

            if (IsFollowingCategory)
            {
                ServiceModel = await _categoryFollowService.FollowedSubCategoriesAsync(e.NewTextValue, new PagingModel { });
            }
            else
            {
                ServiceModel = await _categoryService.SearchAsync(e.NewTextValue, _categoryId, new PagingModel { });
            }

            Items.Clear();

            await base.InitializeAsync();

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
                return gotoProfilePageCommand ?? (gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommandAsync(userName)));
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

        public ICommand SubCategoriesDisplayActionSheetCommand => _SubCategoriesDisplayActionSheetCommand ?? (_SubCategoriesDisplayActionSheetCommand = new Command<short>(async (CategoryId) =>
                        await ExecuteSubCategoriesDisplayActionSheetCommand(CategoryId)));

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("CategoryId")) _categoryId = parameters.GetValue<short>("CategoryId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}
