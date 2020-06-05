using ContestPark.Admin.API.Enums;

namespace ContestPark.Admin.API.Model.Bet
{
    public class BetModel
    {
        public byte BetId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal EarnedCoin { get; set; }

        public decimal EntryFee { get; set; }
        public BalanceTypes BalanceType { get; set; }
    }
}
