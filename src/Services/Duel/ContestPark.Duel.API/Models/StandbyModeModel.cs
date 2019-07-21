using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class StandbyModeModel
    {
        public decimal Bet { get; set; }

        public short SubCategoryId { get; set; }

        public string ConnectionId { get; set; }

        public BalanceTypes BalanceType { get; set; }
    }
}
