using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Signalr.Base;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
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
        private readonly ICategoryService _categoryServices;

        #endregion Private variables

        #region Constructor

        public CategoriesViewModel(ICategoryService categoryServices,
                                   ISignalRServiceBase baseSignalRService,// signalr bağlantısı başlatılması için ekledim
                                   INavigationService navigationService,
                                   IPageDialogService pageDialogService,
                                   IGameService gameService,
                                   IAdMobService adMobService,
                                   IEventAggregator eventAggregator
            ) : base(navigationService, pageDialogService)
        {
            _categoryServices = categoryServices;

            gameService.NavigationService = navigationService;

            EventSubscribe(eventAggregator);
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok

            _baseSignalRService = baseSignalRService;
            _adMobService = adMobService;
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
