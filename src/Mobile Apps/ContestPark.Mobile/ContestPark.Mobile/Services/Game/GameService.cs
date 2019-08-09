using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.CategoryFollow;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.Game
{
    public class GameService : IGameService
    {
        #region Private variables

        private readonly ICategoryFollowService _categoryFollowService;
        private readonly ICategoryService _categoryServices;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPageDialogService _pageDialogService;
        private readonly IPopupNavigation _popupNavigation;

        #endregion Private variables

        #region Constructor

        public GameService(
            ICategoryFollowService categoryFollowService,
            IPageDialogService pageDialogService,
            IEventAggregator eventAggregator,
            IPopupNavigation popupNavigation,
            ICategoryService categoryServices)
        {
            _categoryFollowService = categoryFollowService;
            _pageDialogService = pageDialogService;
            _eventAggregator = eventAggregator;
            _popupNavigation = popupNavigation;
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
        public async Task<bool> PushCategoryDetailViewAsync(short subCategoryId, bool isCategoryOpen)
        {
            if (isCategoryOpen)
                await GoToCategoryDetailViewAsync(subCategoryId);
            else
            {
                bool isUnLock = await OpenSubCategory(subCategoryId);
                if (isUnLock)
                {
                    SubCategoryRefleshEvent();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Alt kategori Long press Alert
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="subCategoryName">Alt kategori adı</param>
        public async Task SubCategoriesDisplayActionSheetAsync(short subCategoryId, string subCategoryName, bool isCategoryOpen)
        {
            if (isCategoryOpen)
            {
                bool isSubCategoryFollowUpStatus = await _categoryFollowService?.IsFollowUpStatusAsync(subCategoryId);

                string selected = await _pageDialogService?.DisplayActionSheetAsync(ContestParkResources.SelectProcess,
                                                                                   ContestParkResources.Cancel,
                                                                                   "",
                                                                                   //buttons
                                                                                   ContestParkResources.FindOpponent,
                                                                                   ContestParkResources.Ranking,
                                                                                   isSubCategoryFollowUpStatus ? ContestParkResources.UnFollow : ContestParkResources.Follow,
                                                                                   ContestParkResources.Share);
                if (string.Equals(selected, ContestParkResources.FindOpponent))
                {
                    await OpenBetPopup(subCategoryId, subCategoryName);
                }
                else if (string.Equals(selected, ContestParkResources.Ranking))
                {
                    await GotoRankingPage(subCategoryId, subCategoryName);
                }
                else if (string.Equals(selected, ContestParkResources.Follow) || string.Equals(selected, ContestParkResources.UnFollow))
                {
                    await SubCategoryFollowProgcess(subCategoryId, isSubCategoryFollowUpStatus);
                }
                else if (string.Equals(selected, ContestParkResources.Share))
                {
                    SubCategoryShare(subCategoryName);
                }
            }
            else
            {
                bool isUnLock = await OpenSubCategory(subCategoryId);
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
        public void SubCategoryShare(string Title)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            string appStoreLink = Device.OnPlatform("", "https://play.google.com/store/apps/details?id=com.contestparkapp.app", "");
#pragma warning restore CS0618 // Type or member is obsolete

            Share.RequestAsync(new ShareTextRequest
            {
                Text = "Social competition platform.",
                Title = "ContestPark",
                Uri = appStoreLink,
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

        private async Task GoToCategoryDetailViewAsync(short subCategoryId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await NavigationService?.NavigateAsync(nameof(CategoryDetailView), new NavigationParameters
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
        private async Task GotoRankingPage(short subCategoryId, string subCategoryName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await NavigationService?.NavigateAsync(nameof(RankingView), new NavigationParameters
                                                {
                                                    { "SubCategoryName", subCategoryName },
                                                    { "SubCategoryId", subCategoryId }
                                                }, useModalNavigation: false);

            IsBusy = false;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private async Task OpenBetPopup(short subCategoryId, string subCategoryName)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await _popupNavigation.PushAsync(new DuelBettingPopupView()
            {
                SubcategoryId = subCategoryId,
                SubcategoryName = subCategoryName,
            });

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
            bool isOk = await _categoryFollowService?.SubCategoryFollowProgcess(subCategoryId, isSubCategoryFollowUpStatus);
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
