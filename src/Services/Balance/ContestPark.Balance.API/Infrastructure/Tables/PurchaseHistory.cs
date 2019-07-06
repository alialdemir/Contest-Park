using ContestPark.Balance.API.Enums;
using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    [Table("PurchaseHistories")]
    public class PurchaseHistory : EntityBase
    {
        [Key]
        public int PurchaseHistoryId { get; set; }

        public string PackageName { get; set; }

        public string ProductId { get; set; }

        public string Token { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public string UserId { get; set; }

        public decimal Amount { get; set; }
        public Platforms Platform { get; set; }
    }
}
