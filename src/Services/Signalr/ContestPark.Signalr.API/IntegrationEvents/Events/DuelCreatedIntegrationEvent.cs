using ContestPark.EventBus.Events;
using ContestPark.Signalr.API.Models;
using System.Collections.Generic;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
{
    public class DuelCreatedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<QuestionModel> Questions { get; set; }

        public int DuelId { get; set; }
        public string FounderUserId { get; }
        public string FounderConnectionId { get; set; }
        public string OpponentUserId { get; }
        public string OpponentConnectionId { get; set; }

        public DuelCreatedIntegrationEvent(int duelId,
                                           string founderUserId,
                                           string founderConnectionId,
                                           string opponentUserId,
                                           string opponentConnectionId,
                                           IEnumerable<QuestionModel> questions)
        {
            DuelId = duelId;
            FounderUserId = founderUserId;
            FounderConnectionId = founderConnectionId;
            OpponentUserId = opponentUserId;
            OpponentConnectionId = opponentConnectionId;
            Questions = questions;
        }
    }
}
