using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("BalanceRankings")]
    public class BalanceRanking : EntityBase
    {
        [Key]
        public int BalanceRankingId { get; set; }

        public short ContestDateId { get; set; }

        public string UserId { get; set; }

        public decimal TotalMoney { get; set; }

        public int TotalGold { get; set; }

        public string DisplayTotalMoney { get; set; }

        public string DisplayTotalGold { get; set; }
    }
}
