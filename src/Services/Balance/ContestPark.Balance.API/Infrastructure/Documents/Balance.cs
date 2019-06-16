using ContestPark.Core.CosmosDb.Models;
using System.Collections.Generic;

namespace ContestPark.Balance.API.Infrastructure.Documents
{
    public class Balance : DocumentBase
    {
        public string UserId { get; set; }
        public virtual ICollection<BalanceAmount> BalanceAmounts { get; set; } = new HashSet<BalanceAmount>();
    }
}