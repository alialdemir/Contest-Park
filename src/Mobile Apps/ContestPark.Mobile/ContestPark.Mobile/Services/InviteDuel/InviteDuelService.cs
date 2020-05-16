using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Views;
using Prism.Navigation;
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
        private readonly IPopupNavigation _popupNavigation;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly decimal[] _goldBets = new decimal[] { 0, 40.00m, 300.00m, 1000.00m, 3000.00m, 4600.00m, 10000.00m };
        private readonly decimal[] _moneyBets = new decimal[] { 1.00m, 3.00m, 4.00m, 5.00m, 6.00m, 10.00m };

        #endregion Private variables

        #region Constructor

        public InviteDuelService(IIdentityService identityService,
                                 IBalanceService balanceService,
                                 ICategoryService categoryService,
                                 IPopupNavigation popupNavigation,
                                 ISettingsService settingsService,
                                 INavigationService navigationService)
        {
            _identityService = identityService;
            _balanceService = balanceService;
            _categoryService = categoryService;
            _popupNavigation = popupNavigation;
            _settingsService = settingsService;
            _navigationService = navigationService;
        }

        #endregion Constructor

        #region Properties

        public byte BalanceTypeIndex { get; set; }

        private int RandomSecond
        {
            get
            {
                return new Random().Next(15, 60);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Belirli aralıklarla ekrana düello daveti isteği gösterir
        /// </summary>
        private void StartTimer(IEnumerable<CategoryModel> categories)
        {
            if (categories == null || !categories.Any())
                return;

            Device.StartTimer(new TimeSpan(0, 0, 0, RandomSecond, 0), () =>
            {
                Device.BeginInvokeOnMainThread(async () => await InviteDuel(categories));

                return false;
            });
        }

        /// <summary>
        /// Random düello daveti gösterir
        /// </summary>
        /// <param name="categories">Kategoriler</param>
        private async Task InviteDuel(IEnumerable<CategoryModel> categories)
        {
            string popupName = CurrentPopupName();
            if (popupName.EndsWith("PopupView"))// Eğer düellodaysa davet göndermesin
            {
                StartTimer(categories);

                return;
            }

            // TODO: random bot user çekerken liste şeklinde çekelim mobile tarafta random davet atsın böylece sunucuya giden request sayısını azaltmış oluruz

            var randomBotUser = await _identityService.GetRandomBotUser();
            if (randomBotUser == null)
            {
                StartTimer(categories);

                return;
            }

            SubCategoryModel subCategory = GetRandomSubCategory(categories);
            if (subCategory == null)
            {
                StartTimer(categories);

                return;
            }

            if (string.IsNullOrEmpty(_settingsService.AuthAccessToken) || _settingsService.CurrentUser.UserId.EndsWith("-bot"))
                return;

            BalanceTypes balanceType = BalanceTypes.Money;

            BalanceTypeIndex += 1;

            if (BalanceTypeIndex > 3)
            {
                BalanceTypeIndex = 0;
                balanceType = BalanceTypes.Gold;
            }

            var InviteModel = new Models.Duel.InviteDuel.InviteModel
            {
                BalanceType = balanceType,
                Bet = await GetRandomBet(balanceType),
                FounderConnectionId = "rnsadjge4",
                FounderFullname = randomBotUser.FullName,
                FounderUserId = randomBotUser.UserId,
                FounderProfilePicturePath = randomBotUser.ProfilePicturePath,
                FounderLanguage = _settingsService.CurrentUser.Language,
                OpponentUserId = _settingsService.CurrentUser.UserId,
                IsOpponentOpenSubCategory = subCategory.IsSubCategoryOpen,
                SubCategoryId = subCategory.SubCategoryId,
                SubCategoryName = subCategory.SubCategoryName,
                SubCategoryPicture = subCategory.PicturePath,
                Description = string.Format(ContestParkResources.IsLookingForAnOpponent, randomBotUser.FullName),
            };

            await _navigationService.NavigateAsync(nameof(AcceptDuelInvitationPopupView), new NavigationParameters
            {
                { "InviteModel", InviteModel }
            }, useModalNavigation: true);

            await Task.Delay(5000);

            StartTimer(categories);
        }

        /// <summary>
        /// Bakiye tipine göre kullanıcının o bakiyesinden düşük bir bahis tutarı verir
        /// </summary>
        /// <param name="balanceType">Bakiye tipi</param>
        /// <returns>Duello bahis tutarı</returns>
        private async Task<decimal> GetRandomBet(BalanceTypes balanceType)
        {
            var balance = await _balanceService.GetBalanceAsync();
            if (balance == null)
                return 0;

            decimal[] bets;
            int index = 0;

            if (balanceType == BalanceTypes.Money)
            {
                bets = _moneyBets
                               .Where(bet => bet <= balance.Money)
                               .ToArray();

                if (bets.Length == 0)
                {
                    return _moneyBets
                                .OrderBy(x => Guid.NewGuid())
                                .FirstOrDefault() * 2;
                }

                index = new Random().Next(0, bets.Length - 1);

                return bets[index] * 2;
            }

            if (balance.Gold < 40.00m)
                return _goldBets.FirstOrDefault();

            var randomBet = _goldBets
                                  .Where(bet => bet != 0 && bet <= balance.Gold)
                                  .OrderBy(x => Guid.NewGuid())
                                  .FirstOrDefault();

            return randomBet * 2;
        }

        /// <summary>
        /// Rastgele kullanıcının açık alt kategorilerinden birini verir
        /// </summary>
        /// <param name="categories">Kategoriler</param>
        /// <returns>Alt kategori</returns>
        private SubCategoryModel GetRandomSubCategory(IEnumerable<CategoryModel> categories)
        {
            return categories
                        .Where(x => x.SubCategories.Any(sc => sc.IsSubCategoryOpen))
                        .OrderBy(x => Guid.NewGuid())
                        .Select(c => c.SubCategories.Where(x => x.IsSubCategoryOpen).OrderBy(x => Guid.NewGuid()).FirstOrDefault())
                        .FirstOrDefault();
        }

        public string CurrentPopupName()
        {
            if (_popupNavigation == null || !_popupNavigation.PopupStack.Any())
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
            get { return new Command<IEnumerable<CategoryModel>>(StartTimer); }
        }

        #endregion Commands
    }
}
