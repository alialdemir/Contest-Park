using ContestPark.Balance.API.Enums;
using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    [Table("BalanceHistories")]
    public class BalanceHistory : EntityBase
    {
        [Key]
        public int BalanceHistoryId { get; set; }

        public BalanceHistoryTypes BalanceHistoryType { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public string UserId { get; set; }
    }
}
