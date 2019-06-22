using ContestPark.Balance.API.Enums;

namespace ContestPark.Balance.API.Models
{
    public class ChangeBalanceModel
    {
        public string UserId { get; set; }
        public BalanceTypes BalanceType { get; set; }
        public BalanceHistoryTypes BalanceHistoryType { get; set; }
        public decimal Amount { get; set; }
    }
}