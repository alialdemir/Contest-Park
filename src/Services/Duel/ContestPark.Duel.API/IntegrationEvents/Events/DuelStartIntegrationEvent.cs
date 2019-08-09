using ContestPark.Core.Enums;
using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class DuelStartIntegrationEvent : IntegrationEvent
    {
        public short SubCategoryId { get; set; }

        public decimal Bet { get; set; }

        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public string FounderConnectionId { get; set; }

        public BalanceTypes BalanceType { get; }

        public Languages FounderLanguage { get; }

        public Languages OpponentLanguage { get; }

        public string OpponentConnectionId { get; set; }

        public DuelStartIntegrationEvent(short subCategoryId,
                                         decimal bet,
                                         string founderUserId,
                                         string opponentUserId,
                                         string founderConnectionId,
                                         string opponentConnectionId,
                                         BalanceTypes balanceType,
                                         Languages founderLanguage,
                                         Languages opponentLanguage)
        {
            SubCategoryId = subCategoryId;
            Bet = bet;
            FounderUserId = founderUserId;
            OpponentUserId = opponentUserId;
            FounderConnectionId = founderConnectionId;
            OpponentConnectionId = opponentConnectionId;
            BalanceType = balanceType;
            FounderLanguage = founderLanguage;
            OpponentLanguage = opponentLanguage;
            OpponentUserId = opponentUserId;
        }
    }
}
