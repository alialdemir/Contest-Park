using ContestPark.Balance.API.Enums;
using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Balance.API.Infrastructure.Documents
{
    public class PurchaseHistory : DocumentBase
    {
        public string PackageName { get; set; }

        public string ProductId { get; set; }

        public string Token { get; set; }

        public BalanceTypes BalanceType { get; set; }

        public string UserId { get; set; }

        public decimal Amount { get; set; }
        public Platforms Platform { get; set; }
    }
}