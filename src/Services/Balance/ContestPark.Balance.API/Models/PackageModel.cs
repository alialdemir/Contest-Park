using ContestPark.Balance.API.Enums;

namespace ContestPark.Balance.API.Models
{
    public class PackageModel
    {
        public decimal Amount { get; set; }
        public BalanceTypes BalanceType { get; set; }

        public bool IsSpecialOffer { get; set; }
    }
}
