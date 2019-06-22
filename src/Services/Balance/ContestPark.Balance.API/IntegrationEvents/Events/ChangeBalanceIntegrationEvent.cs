using ContestPark.Balance.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Balance.API.IntegrationEvents.Events
{
    public class ChangeBalanceIntegrationEvent : IntegrationEvent
    {
        public decimal Amount { get; set; }
        public string UserId { get; set; }

        public BalanceTypes BalanceType { get; set; }
        public BalanceHistoryTypes BalanceHistoryType { get; set; }

        public ChangeBalanceIntegrationEvent(decimal amount, string userId, BalanceTypes balanceType, BalanceHistoryTypes balanceHistoryType)
        {
            Amount = amount;
            UserId = userId;
            BalanceType = balanceType;
            BalanceHistoryType = balanceHistoryType;
        }
    }
}