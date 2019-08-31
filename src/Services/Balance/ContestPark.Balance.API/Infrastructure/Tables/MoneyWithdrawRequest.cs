using ContestPark.Balance.API.Enums;
using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Balance.API.Infrastructure.Tables
{
    [Table("MoneyWithdrawRequests")]
    public class MoneyWithdrawRequest : EntityBase
    {
        [Key]
        public int MoneyWithdrawRequestId { get; set; }

        public string UserId { get; set; }

        public decimal Amount { get; set; }

        public string IbanNo { get; set; }

        public string FullName { get; set; }

        public Status Status { get; set; }
    }
}
