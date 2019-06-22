using ContestPark.Balance.API.Enums;

namespace ContestPark.Balance.API.Models
{
    public class BalanceModel
    {
        public BalanceTypes BalanceType { get; set; }
        public decimal Amount { get; set; }
    }
}