using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Cp;
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

        private readonly ICategoryServices _categoryServices;

        private readonly ICpService _cpService;

        private readonly ISignalRServiceBase _baseSignalRService;

        #endregion Private variables

        #region Constructor

        public CategoriesViewModel(
            ICategoryServices categoryServices,
            ISignalRServiceBase baseSignalRService,// signalr bağlantısı başlatılması için ekledim
            ICpService cpService,
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            IGameService gameService,
            IEventAggregator eventAggregator) : base(navigationService, pageDialogService)
        {
            Title = ContestParkResources.Categories;
            _cpService = cpService;
            _categoryServices = categoryServices;

            gameService.NavigationService = navigationService;

            EventSubscribe(eventAggregator);
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok

            _baseSignalRService = baseSignalRService;
        }

        #endregion Constructor

        #region Properties

        public int SeeAllSubCateogryId { get; set; } = 0;

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        private string _userCoins = "0";

        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public string UserCoins
        {
            get
            {
                return _userCoins;
            }
            set
            {
                _userCoins = value;
                RaisePropertyChanged(() => UserCoins);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!_baseSignalRService.IsConnect)
                await _baseSignalRService.Init();

            SetUserGoldCommand.Execute(null);

            //TODO: Kategorileri sayfala
            ServiceModel = await _categoryServices.CategoryListAsync(ServiceModel);

            await base.InitializeAsync();
            IsBusy = false;
        }

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        /// <returns></returns>
        private async Task SetUserGoldAsync()
        {
            int userGold = await _cpService.GetTotalCpByUserIdAsync();
            if (userGold > 0) UserCoins = userGold.ToString("##,#").Replace(",", ".");
            else UserCoins = userGold.ToString();
        }

        /// <summary>
        /// Event dinleme
        /// </summary>
        private void EventSubscribe(IEventAggregator eventAggregator) => eventAggregator.GetEvent<SubCategoryRefleshEvent>()
                .Subscribe(() => RefleshCommand.Execute(null));

        private async Task ExecutGoToCategorySearchPageCommand(int CategoryId = 0)
        {
            await PushNavigationPageAsync($"{nameof(CategorySearchView)}", new NavigationParameters
                                                {
                                                    { "CategoryId", CategoryId }
                                                }, useModalNavigation: false);
        }

        #endregion Methods

        #region Commands

        private ICommand _goToCategorySearchPageCommand;
        public ICommand GoToCategorySearchPageCommand => _goToCategorySearchPageCommand ?? (_goToCategorySearchPageCommand = new Command<int>(async (CategoryId) => await ExecutGoToCategorySearchPageCommand(CategoryId)));

        public ICommand SetUserGoldCommand => new Command(async () => await SetUserGoldAsync());

        #endregion Commands
    }
}