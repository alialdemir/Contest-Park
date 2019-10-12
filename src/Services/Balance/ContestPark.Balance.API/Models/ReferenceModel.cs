using ContestPark.Balance.API.Enums;

namespace ContestPark.Balance.API.Models
{
    public class ReferenceModel
    {
        public BalanceTypes BalanceType { get; set; }

        public decimal Amount { get; set; }
    }
}
