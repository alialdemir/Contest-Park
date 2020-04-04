using ContestPark.Mobile.Enums;
using static ContestPark.Mobile.ViewModels.DuelStartingPopupViewModel;

namespace ContestPark.Mobile.Models.Duel
{
    public class SelectedBetModel : SelectedSubCategoryModel
    {
        public decimal Bet { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public StandbyModes StandbyMode { get; set; }
    }
}
