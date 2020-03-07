using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class InviteDuelModel
    {
        public string OpponentUserId { get; set; }

        public short SubCategoryId { get; set; }

        public decimal Bet { get; set; }

        public BalanceTypes BalanceType { get; set; }
        public string FounderConnectionId { get; set; }
    }
}
