using ContestPark.Balance.API.Enums;

namespace ContestPark.Balance.API.Infrastructure.Documents
{
    public class BalanceAmount
    {
        public decimal Amount { get; set; }
        public BalanceTypes BalanceType { get; set; }
    }
}