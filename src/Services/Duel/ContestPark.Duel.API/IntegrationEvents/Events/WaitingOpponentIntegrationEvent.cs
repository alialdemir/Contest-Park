using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Category.API.IntegrationEvents.Events
{
    public class WaitingOpponentIntegrationEvent : IntegrationEvent
    {
        public decimal Bet { get; }

        public short SubCategoryId { get; }

        public string UserId { get; }

        public string ConnectionId { get; }

        public BalanceTypes BalanceType { get; }

        public WaitingOpponentIntegrationEvent(string userId,
                                               string connectionId,
                                               short subCategoryId,
                                               decimal bet,
                                               BalanceTypes balanceType)
        {
            UserId = userId;
            ConnectionId = connectionId;
            SubCategoryId = subCategoryId;
            Bet = bet;
            BalanceType = balanceType;
        }
    }
}
