using ContestPark.Balance.API.Enums;
using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Balance.API.Infrastructure.Documents
{
    public class BalanceHistory : DocumentBase
    {
        public BalanceHistoryTypes BalanceHistoryType { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public string UserId { get; set; }
    }
}