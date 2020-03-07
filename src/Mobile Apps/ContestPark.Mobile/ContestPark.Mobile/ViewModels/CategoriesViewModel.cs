using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Signalr.Base;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CategoriesViewModel : ViewModelBase<CategoryModel>
    {
        #region Private variables

        private readonly ISignalRServiceBase _baseSignalRService;
        private readonly IGameService _gameService;
        private readonly IAdMobService _adMobService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IDuelSignalRService _duelSignalRService;
        private readonly ICategoryService _categoryServices;

        #endregion Private variables

        #region Constructor

        public CategoriesViewModel(ICategoryService categoryServices,
                                   ISignalRServiceBase baseSignalRService,// signalr bağlantısı başlatılması için ekledim
                                   INavigationService navigationService,
                                   IPageDialogService pageDialogService,
                                   IGameService gameService,
                                   IAdMobService adMobService,
                                   IPopupNavigation popupNavigation,
                                   IAnalyticsService analyticsService,
                                   IDuelSignalRService duelSignalRService,
                                   IEventAggregator eventAggregator
            ) : base(navigationService, pageDialogService, popupNavigation: popupNavigation)
        {
            Title = ContestParkResources.Categories;

            _categoryServices = categoryServices;

            gameService.NavigationService = navigationService;

            EventSubscribe(eventAggregator);
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok

            _baseSignalRService = baseSignalRService;
            _gameService = gameService;
            _adMobService = adMobService;
            _analyticsService = analyticsService;
            _duelSignalRService = duelSignalRService;

            #region Skeleton loading

            var categories = new CategoryModel
            {
                IsBusy = true,
                SubCategories = new System.Collections.Generic.List<SubCategoryModel>
                {
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                }
            };

            Items.Add(categories);
            Items.Add(categories);
            Items.Add(categories);
            Items.Add(categories);
            Items.Add(categories);

            #endregion Skeleton loading
        }

        #endregion Constructor

        #region Properties

        public short SeeAllSubCateogryId { get; set; } = 0;

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ConnectToSignalr.Execute(null);

            //TODO: Kategorileri sayfala
            ServiceModel = await _categoryServices.CategoryListAsync(ServiceModel);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Event dinleme
        /// </summary>
        private void EventSubscribe(IEventAggregator eventAggregator) => eventAggregator.GetEvent<SubCategoryRefleshEvent>()
                .Subscribe(() => RefreshCommand.Execute(null));

        /// <summary>
        /// Kategori search sayfasına gider
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private void ExecutGoToCategorySearchPageCommand(short categoryId = 0)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (categoryId != 0)
            {
                string subCategoryName = Items.FirstOrDefault(x => x.CategoryId == categoryId).CategoryName;

                _analyticsService.SendEvent("Kategori", "Tümünü Gör", subCategoryName);
            }

            PushNavigationPageAsync($"{nameof(SearchView)}", new NavigationParameters
                                                {
                                                    { "CategoryId", categoryId }
                                                }, useModalNavigation: false);

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategoriye uzun basınca ActionSheet gösterir
        /// </summary>
        private async Task AddLongPressed(SubCategoryModel subCategory)
        {
            if (IsBusy || subCategory == null)
                return;

            IsBusy = true;

            await _gameService?.SubCategoriesDisplayActionSheetAsync(new SelectedSubCategoryModel
            {
                SubcategoryId = subCategory.SubCategoryId,
                SubcategoryName = subCategory.SubCategoryName,
                SubCategoryPicturePath = subCategory.PicturePath
            }, subCategory.IsCategoryOpen);

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategoriye tıklanınca kategori detaya gider
        /// </summary>
        private async Task AddSingleTap(SubCategoryModel subCategory)
        {
            if (IsBusy || subCategory == null)
                return;

            IsBusy = true;

            await _gameService?.PushCategoryDetailViewAsync(subCategory.SubCategoryId,
                                                            subCategory.IsCategoryOpen,
                                                            subCategory.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Düello davetleri buraya düşer
        /// </summary>
        private void OnInviteModel(object sender, InviteModel e)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string popupName = CurrentPopupName();
            if (popupName != nameof(QuestionExpectedPopupView) && popupName != nameof(QuestionPopupView) && popupName != nameof(AcceptDuelInvitationPopupView))
            {
                PushPopupPageAsync(new AcceptDuelInvitationPopupView
                {
                    InviteModel = (InviteModel)sender
                });
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand _goToCategorySearchPageCommand;
        public ICommand GoToCategorySearchPageCommand => _goToCategorySearchPageCommand ?? (_goToCategorySearchPageCommand = new Command<short>((categoryId) => ExecutGoToCategorySearchPageCommand(categoryId)));

        private ICommand ConnectToSignalr => new Command(() =>
       {
           Device.BeginInvokeOnMainThread(async () =>
           {
               if (!_baseSignalRService.IsConnect)
                   await _baseSignalRService.Init();

               // Düello daveti dinleme
               _duelSignalRService.InviteDuelEventHandler += OnInviteModel;
               _duelSignalRService.InviteDuel();
           });
       });

        private ICommand _pushCategoryDetailViewCommand;

        public ICommand PushCategoryDetailViewCommand
        {
            get
            {
                return _pushCategoryDetailViewCommand ?? (_pushCategoryDetailViewCommand = new Command<SubCategoryModel>(async (subCategory) => await AddSingleTap(subCategory)));
            }
        }

        private ICommand _subCategoriesDisplayActionSheetCommand;

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand SubCategoriesDisplayActionSheetCommand
        {
            get
            {
                return _subCategoriesDisplayActionSheetCommand ?? (_subCategoriesDisplayActionSheetCommand = new Command<SubCategoryModel>(async (subCategory) => await AddLongPressed(subCategory)));
            }
        }

        #endregion Commands
    }
}
