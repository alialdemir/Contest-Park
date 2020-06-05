using ContestPark.Admin.API.Enums;
using Dapper;

namespace ContestPark.Admin.API.Infrastructure.Tables
{
    [Table("Bets")]
    public class Bet
    {
        [Key]
        public byte BetId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal EarnedCoin { get; set; }

        public decimal EntryFee { get; set; }
        public BalanceTypes BalanceType { get; set; }
    }
}
