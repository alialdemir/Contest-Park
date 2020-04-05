using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Services.Game
{
    public class GameService : IGameService
    {
        #region Private variables

        private readonly ICategoryService _categoryServices;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
        private readonly IAnalyticsService _analyticsService;

        #endregion Private variables

        #region Constructor

        public GameService(
            IPageDialogService pageDialogService,
            IEventAggregator eventAggregator,
            IAnalyticsService analyticsService,
            ICategoryService categoryServices)
        {
            _pageDialogService = pageDialogService;
            _eventAggregator = eventAggregator;
            _analyticsService = analyticsService;
            _categoryServices = categoryServices;
        }

        #endregion Constructor

        #region Property

        public bool IsBusy { get; set; }
        public INavigationService NavigationService { get; set; }

        #endregion Property

        #region Methods

        /// <summary>
        /// Kategori kilidi açık ise EnterPage gider değilse alert çıkarır kilidi aç der
        /// </summary>
        /// <param name="subCategoryModel"></param>
        /// <returns>Kilitli ise kilidi açsınmı isteğini sorar açsın derse true döndürür</returns>
        public async Task<bool> PushCategoryDetailViewAsync(short subCategoryId, bool isCategoryOpen, string subCategoryName)
        {
            if (isCategoryOpen)
                GoToCategoryDetailViewAsync(subCategoryId, subCategoryName);
            else
            {
                return await OpenSubCategoryProgcess(subCategoryId);
            }

            return false;
        }

        /// <summary>
        /// Alt kategori kilitli ise kategori kilidi açılsın mı dialog çıkarır
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Kilit açılmış ise true açılmamış ise false</returns>
        public async Task<bool> OpenSubCategoryProgcess(short subCategoryId)
        {
            bool isUnLock = await OpenSubCategory(subCategoryId);
            if (isUnLock)
            {
                SubCategoryRefleshEvent();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Alt kategori Long press Alert
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="subCategoryName">Alt kategori adı</param>
        public async Task SubCategoriesDisplayActionSheetAsync(SelectedSubCategoryModel selectedSubCategory, bool isCategoryOpen)
        {
            if (isCategoryOpen)
            {
                bool isSubCategoryFollowUpStatus = await _categoryServices?.IsFollowUpStatusAsync(selectedSubCategory.SubcategoryId);

                string selected = await _pageDialogService?.DisplayActionSheetAsync(ContestParkResources.SelectProcess,
                                                                                   ContestParkResources.Cancel,
                                                                                   null,
                                                                                   //buttons
                                                                                   ContestParkResources.FindOpponent,
                                                                                   ContestParkResources.Ranking,
                                                                                   isSubCategoryFollowUpStatus ? ContestParkResources.UnFollow : ContestParkResources.Follow,
                                                                                   ContestParkResources.Share);
                if (string.Equals(selected, ContestParkResources.FindOpponent))
                {
                    OpenBetPopup(selectedSubCategory);
                }
                else if (string.Equals(selected, ContestParkResources.Ranking))
                {
                    await GotoRankingPage(selectedSubCategory.SubcategoryId);
                }
                else if (string.Equals(selected, ContestParkResources.Follow) || string.Equals(selected, ContestParkResources.UnFollow))
                {
                    await SubCategoryFollowProgcess(selectedSubCategory.SubcategoryId, isSubCategoryFollowUpStatus);
                }
                else if (string.Equals(selected, ContestParkResources.Share))
                {
                    SubCategoryShare();
                }
            }
            else
            {
                bool isUnLock = await OpenSubCategory(selectedSubCategory.SubcategoryId);
                if (isUnLock)
                {
                    SubCategoryRefleshEvent();
                }
            }
        }

        /// <summary>
        /// Alt kategoriyi sosyal medyada paylaş
        /// </summary>
        /// <param name="Title">Alt kategori adı</param>
        public void SubCategoryShare()
        {
            Share.RequestAsync(new ShareTextRequest
            {
                Text = "İlgi alanlarınıza göre soru sorarak yarıştırıp, para kazandıran bilgi yarışması. Hemen İndir!",
                Title = "ContestPark",
                Uri = "http://indir.contestpark.com",
            });
        }

        #endregion Methods

        #region Private methods

        /// <summary>
        /// Altın satın almak istermisiniz sorusunu sorar onaylarsa Contest Store sayfasına gider
        /// </summary>
        private async Task BuyDisplayAlertAsync()
        {
            bool isOk = await _pageDialogService?.DisplayAlertAsync(ContestParkResources.NoGold,
                                                                   ContestParkResources.YouDoNotHaveASufficientAmountOfGoldToOpenThisCategory,
                                                                   ContestParkResources.Buy,
                                                                   ContestParkResources.Cancel);

            if (isOk) await NavigationService?.NavigateAsync(nameof(ContestStoreView), useModalNavigation: false);
        }

        private void GoToCategoryDetailViewAsync(short subCategoryId, string subCategoryName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            _analyticsService.SendEvent("Kategori", "Kategori Detayı", subCategoryName);

            NavigationService?.NavigateAsync(nameof(CategoryDetailView), new NavigationParameters
                                                {
                                                    { "SubCategoryId", subCategoryId }
                                                }, useModalNavigation: false);

            IsBusy = false;
        }

        /// <summary>
        /// İlgili kategorinin sıralama sayfasına gider
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="subCategoryName">Alt kategori adı</param>
        private async Task GotoRankingPage(short subCategoryId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await NavigationService?.NavigateAsync(nameof(RankingView), new NavigationParameters
                                                {
                                                    { "SubCategoryId", subCategoryId }
                                                }, useModalNavigation: false);

            IsBusy = false;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private void OpenBetPopup(SelectedSubCategoryModel selectedSubCategory)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            NavigationService?.NavigateAsync(nameof(DuelBettingPopupView), new NavigationParameters
            {
                { "SelectedSubCategory", selectedSubCategory }
            }, useModalNavigation: true);

            IsBusy = false;
        }

        /// <summary>
        /// Kilitli kategorinin kilidini açmak ister misiniz mesajı
        /// </summary>
        /// <param name="subCategoryId"></param>
        /// <returns></returns>
        private async Task<bool> OpenSubCategory(short subCategoryId)
        {
            bool isUnLock = await _pageDialogService?.DisplayAlertAsync(ContestParkResources.UnLock + "?",
                                                                   ContestParkResources.CategoryLocked,
                                                                   ContestParkResources.UnLock,
                                                                   ContestParkResources.Cancel);
            if (!isUnLock)
                return isUnLock;

            var response = await _categoryServices?.OpenCategoryAsync(subCategoryId);
            if (response?.Error?.ErrorStatuCode == ErrorStatuCodes.YouCanNotUnlockTheFreeCategory)
                await BuyDisplayAlertAsync();
            else if (!response.IsSuccess)
            {
                await _pageDialogService.DisplayAlertAsync(ContestParkResources.Error,
                                                           response.Error.ErrorMessage,
                                                           ContestParkResources.Okay);
            }

            return isUnLock;
        }

        /// <summary>
        /// Alt kategori takip et veya takip bırakma işlemi yapar parametreden gelen isSubCategoryFollowUpStatus true ise takibi bırakır false ise
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="isSubCategoryFollowUpStatus">O anki alt kategori takip etme durumu</param>
        private async Task SubCategoryFollowProgcess(short subCategoryId, bool isSubCategoryFollowUpStatus)
        {
            bool isOk = await _categoryServices?.SubCategoryFollowProgcess(subCategoryId, isSubCategoryFollowUpStatus);
            if (isOk) SubCategoryRefleshEvent();
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
