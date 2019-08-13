using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Duel
{
    public class StandbyModeModel
    {
        public decimal Bet { get; set; }

        public short SubCategoryId { get; set; }

        public string ConnectionId { get; set; }

        public BalanceTypes BalanceType { get; set; }
    }
}
