using ContestPark.Core.Enums;
using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class WaitingOpponentIntegrationEvent : IntegrationEvent
    {
        public decimal Bet { get; }

        public short SubCategoryId { get; }

        public string UserId { get; }

        public string ConnectionId { get; }

        public BalanceTypes BalanceType { get; }

        public Languages Language { get; set; }

        public WaitingOpponentIntegrationEvent(string userId,
                                               string connectionId,
                                               short subCategoryId,
                                               decimal bet,
                                               BalanceTypes balanceType,
                                               Languages language)
        {
            UserId = userId;
            ConnectionId = connectionId;
            SubCategoryId = subCategoryId;
            Bet = bet;
            BalanceType = balanceType;
            Language = language;
        }
    }
}
