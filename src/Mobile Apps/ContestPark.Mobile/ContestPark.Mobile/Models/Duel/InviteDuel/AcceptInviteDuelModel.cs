using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Duel.InviteDuel
{
    public class AcceptInviteDuelModel
    {
        public short SubCategoryId { get; set; }
        public decimal Bet { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public string FounderUserId { get; set; }
        public string FounderConnectionId { get; set; }
        public Languages FounderLanguage { get; set; }
        public string OpponentConnectionId { get; set; }
    }
}
