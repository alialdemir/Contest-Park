using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Game
{
    public class GameService : IGameService
    {
        #region Private variables

        private readonly IPageDialogService _pageDialogService;
        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructor

        public GameService(
            IPageDialogService pageDialogService,
            IEventAggregator eventAggregator)
        {
            _pageDialogService = pageDialogService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Property

        public INavigationService NavigationService { get; set; }

        public bool IsBusy { get; set; }

        #endregion Property

        #region Methods

        /// <summary>
        /// Kategori kilidi açık ise EnterPage gider değilse alert çıkarır kilidi aç der
        /// </summary>
        /// <param name="subCategoryModel"></param>
        /// <returns>Kilitli ise kilidi açsınmı isteğini sorar açsın derse true döndürür</returns>
        public async Task<bool> PushCategoryDetailViewAsync(SubCategoryModel subCategoryModel)
        {
            if (subCategoryModel.IsCategoryOpen) await GoToCategoryDetailViewAsync(subCategoryModel.SubCategoryName, subCategoryModel.PicturePath, subCategoryModel.SubCategoryId);
            else
            {
                bool isUnLock = await OpenSubCategory(subCategoryModel.SubCategoryId);
                if (isUnLock)
                {
                    SubCategoryRefleshEvent();
                    return true;
                }
            }
            return false;
        }

        #endregion Methods

        #region Private methods

        private async Task GoToCategoryDetailViewAsync(string subCategoryName, string subCategoryPicturePath, int subCategoryId)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await NavigationService?.NavigateAsync(nameof(CategoryDetailView), new NavigationParameters
                                                {
                                                    { "SubCategoryName", subCategoryName },
                                                    { "SubCategoryPicturePath", subCategoryPicturePath },
                                                    { "SubCategoryId", subCategoryId }
                                                }, useModalNavigation: false);
            IsBusy = false;
        }

        /// <summary>
        /// Kilitli kategorinin kilidini açmak ister misiniz mesajı
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns></returns>
        private async Task<bool> OpenSubCategory(int subCategoryId)
        {
            bool isOk = await _pageDialogService?.DisplayAlertAsync(ContestParkResources.UnLock + "?",
                                                                   ContestParkResources.CategoryLocked,
                                                                   ContestParkResources.UnLock,
                                                                   ContestParkResources.Cancel);
            if (!isOk)
                return isOk;
            bool isUnLock = true; //////////////await _openSubCategoryService?.OpenCategoryAsync(subCategoryId);
            if (!isUnLock) await BuyDisplayAlertAsync();
            return isUnLock;
        }

        /// <summary>
        /// Altın satın almak istermisiniz sorusunu sorar onaylarsa Contest Store sayfasına gider
        /// </summary>
        private async Task BuyDisplayAlertAsync()
        {
            bool isOk = await _pageDialogService?.DisplayAlertAsync(ContestParkResources.NoGold,
                                                                   ContestParkResources.YouDoNotHaveASufficientAmountOfGoldToOpenThisCategory,
                                                                   ContestParkResources.Buy,
                                                                   ContestParkResources.Cancel);

            if (isOk) await NavigationService?.NavigateAsync(/*nameof(ContestStorePage)*/"ContestStorePage", useModalNavigation: false);
        }

        /// <summary>
        /// Categories sayfasındaki dataları yenilemek için reflesh eventi tetikler
        /// </summary>
        private void SubCategoryRefleshEvent()
        {
            _eventAggregator
                         .GetEvent<SubCategoryRefleshEvent>()
                         .Publish();
        }

        #endregion Private methods
    }
}