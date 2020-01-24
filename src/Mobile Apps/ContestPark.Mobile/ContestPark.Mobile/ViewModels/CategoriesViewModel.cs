using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Signalr.Base;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
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
        private readonly IAdMobService _adMobService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ICategoryService _categoryServices;

        #endregion Private variables

        #region Constructor

        public CategoriesViewModel(ICategoryService categoryServices,
                                   ISignalRServiceBase baseSignalRService,// signalr bağlantısı başlatılması için ekledim
                                   INavigationService navigationService,
                                   IPageDialogService pageDialogService,
                                   IGameService gameService,
                                   IAdMobService adMobService,
                                   IAnalyticsService analyticsService,
                                   IEventAggregator eventAggregator
            ) : base(navigationService, pageDialogService)
        {
            Title = ContestParkResources.Categories;

            _categoryServices = categoryServices;

            gameService.NavigationService = navigationService;

            EventSubscribe(eventAggregator);
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok

            _baseSignalRService = baseSignalRService;
            _adMobService = adMobService;
            _analyticsService = analyticsService;
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

        #endregion Methods

        #region Commands

        private ICommand _goToCategorySearchPageCommand;
        public ICommand GoToCategorySearchPageCommand => _goToCategorySearchPageCommand ?? (_goToCategorySearchPageCommand = new Command<short>((categoryId) => ExecutGoToCategorySearchPageCommand(categoryId)));

        private ICommand ConnectToSignalr => new Command(() =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!_baseSignalRService.IsConnect)
                    _baseSignalRService.Init();
            });
        });

        #endregion Commands
    }
}
