using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class SelectSubCategoryViewModel : ViewModelBase<CategoryModel>
    {
        #region Private variables

        private readonly ICategoryService _categoryService;
        private readonly IGameService _gameService;

        #endregion Private variables

        #region Constructor

        public SelectSubCategoryViewModel(ICategoryService categoryService,
                                          IGameService gameService,

                                          INavigationService navigationService) : base(navigationService: navigationService)
        {
            _categoryService = categoryService;
            _gameService = gameService;
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok
        }

        #endregion Constructor

        #region Properties

        private string OpponentUserId { get; set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            ServiceModel = await _categoryService.CategoryListAsync(ServiceModel);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private void ExecuteduelOpenPanelCommandAsync(SubCategoryModel subCategory)
        {
            if (IsBusy || subCategory == null)
                return;

            IsBusy = true;

            GotoBackCommand.Execute(true);

            if (subCategory.IsSubCategoryOpen)
            {
                var selectedSubCategory = new SelectedSubCategoryModel
                {
                    SubcategoryId = subCategory.SubCategoryId,
                    SubCategoryName = subCategory.SubCategoryName,
                    SubCategoryPicturePath = subCategory.PicturePath,
                    OpponentUserId = OpponentUserId
                };

                PushModalAsync(nameof(DuelBettingPopupView), new NavigationParameters
                        {
                            { "SelectedSubCategory", selectedSubCategory },
                        });
            }
            else
            {
                _gameService.OpenSubCategoryProgcess(subCategory.SubCategoryId);
            }

            IsBusy = false;
        }

        public override Task GoBackAsync(INavigationParameters parameters = null, bool? useModalNavigation = false)
        {
            return base.GoBackAsync(parameters, useModalNavigation: true);
        }

        #endregion Methods

        #region Commands

        private ICommand _duelOpenPanelCommand;

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public ICommand DuelOpenPanelCommand
        {
            get { return _duelOpenPanelCommand ?? (_duelOpenPanelCommand = new Command<SubCategoryModel>((subCategory) => ExecuteduelOpenPanelCommandAsync(subCategory))); }
        }

        #endregion Commands

        #region Navgation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("OpponentUserId"))
                OpponentUserId = parameters.GetValue<string>("OpponentUserId");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navgation
    }
}
