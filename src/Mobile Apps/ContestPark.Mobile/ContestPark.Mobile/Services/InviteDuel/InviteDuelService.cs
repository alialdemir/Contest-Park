using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Duel.InviteDuel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.Services.InviteDuel
{
    public class InviteDuelService : IInviteDuelService
    {
        #region Private variables

        private readonly IIdentityService _identityService;
        private readonly IBalanceService _balanceService;
        private readonly ICategoryService _categoryService;
        private readonly ISettingsService _settingsService;
        private readonly IPopupNavigation _popupNavigation;

        private readonly decimal[] _goldBets = new decimal[] { 80.00m, 600.00m, 2000.00m, 6000.00m, 9200.00m, 20000.00m };
        private readonly decimal[] _moneyBets = new decimal[] { 2.00m, 6.00m, 8.00m, 10.00m, 12.00m, 20.00m };

        #endregion Private variables

        #region Constructor

        public InviteDuelService(IIdentityService identityService,
                                 IBalanceService balanceService,
                                 ICategoryService categoryService,
                                 ISettingsService settingsService,
                                 IPopupNavigation _popupNavigation)
        {
            _identityService = identityService;
            _balanceService = balanceService;
            _categoryService = categoryService;
            _settingsService = settingsService;
            this._popupNavigation = _popupNavigation;
        }

        #endregion Constructor

        #region Properties

        public int RandomSecond
        {
            get
            {
                return 1000;// new Random().Next(5000, 20000);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Belirli aralıklarla ekrana düello daveti isteği gösterir
        /// </summary>
        private void StartTimer()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(RandomSecond), () =>
            {
                InviteDuel().Wait();

                return false;
            });
        }

        private async Task InviteDuel()
        {
            string popupName = CurrentPopupName();
            if (popupName == nameof(QuestionExpectedPopupView)
                || popupName == nameof(QuestionPopupView)
                || popupName == nameof(AcceptDuelInvitationPopupView))// Eğer düellodaysa davet göndermesin
            {
                StartTimer();
                return;
            }

            var randomBotUser = await _identityService.GetRandomBotUser();
            if (randomBotUser == null)
            {
                StartTimer();
                return;
            }

            var categories = await _categoryService.CategoryListAsync(new PagingModel { PageSize = 9999 });
            if (categories == null || !categories.Items.Any())
            {
                StartTimer();
                return;
            }

            SubCategoryModel subCategory = GetRandomSubCategory(categories.Items);
            if (subCategory == null)
            {
                StartTimer();
                return;
            }

            BalanceTypes balanceType = BalanceTypes.Gold;

            await _popupNavigation.PushAsync(new AcceptDuelInvitationPopupView
            {
                InviteModel = new InviteModel
                {
                    BalanceType = balanceType,
                    Bet = await GetRandomBet(balanceType),
                    FounderConnectionId = "bot",
                    FounderFullname = randomBotUser.FullName,
                    FounderUserId = randomBotUser.UserId,
                    FounderProfilePicturePath = randomBotUser.ProfilePicturePath,
                    FounderLanguage = _settingsService.CurrentUser.Language,
                    OpponentUserId = _settingsService.CurrentUser.UserId,
                    IsOpponentOpenSubCategory = subCategory.IsCategoryOpen,
                    SubCategoryId = subCategory.SubCategoryId,
                    SubCategoryName = subCategory.SubCategoryName,
                    SubCategoryPicture = subCategory.PicturePath,
                }
            });

            StartTimer();
        }

        /// <summary>
        /// Bakiye tipine göre kullanıcının o bakiyesinden düşük bir bahis tutarı verir
        /// </summary>
        /// <param name="balanceType">Bakiye tipi</param>
        /// <returns>Duello bahis tutarı</returns>
        private async Task<decimal> GetRandomBet(BalanceTypes balanceType)
        {
            var balance = await _balanceService.GetBalanceAsync();

            decimal[] bets;
            int index;

            if (balanceType == BalanceTypes.Money)
            {
                bets = _moneyBets
                               .Where(bet => bet <= balance.Money)
                               .ToArray();

                index = new Random().Next(0, bets.Length - 1);

                return bets[index];
            }

            bets = _goldBets
                          .Where(bet => bet <= balance.Gold)
                          .ToArray();

            index = new Random().Next(0, bets.Length - 1);

            return bets[index];
        }

        /// <summary>
        /// Rastgele kullanıcının açık alt kategorilerinden birini verir
        /// </summary>
        /// <param name="categories">Kategoriler</param>
        /// <returns>Alt kategori</returns>
        private SubCategoryModel GetRandomSubCategory(IEnumerable<CategoryModel> categories)
        {
            return categories
                        .Where(x => x.SubCategories.Any(sc => sc.IsCategoryOpen))
                        .OrderBy(x => Guid.NewGuid())
                        .Select(c => c.SubCategories.Where(x => x.IsCategoryOpen).OrderBy(x => Guid.NewGuid()).FirstOrDefault())
                        .FirstOrDefault();
        }

        private string CurrentPopupName()
        {
            if (_popupNavigation == null || _popupNavigation.PopupStack.Count == 0)
                return string.Empty;

            PopupPage popupPage = _popupNavigation.PopupStack.FirstOrDefault();
            if (popupPage == null)
                return string.Empty;

            return popupPage.GetType().Name;
        }

        #endregion Methods

        #region Commands

        public ICommand InviteDuelCommand
        {
            get { return new Command(StartTimer); }
        }

        #endregion Commands
    }
}
