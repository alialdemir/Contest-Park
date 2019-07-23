using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class DuelCreatedIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<QuestionModel> Questions { get; set; }
        public int DuelId { get; set; }

        public DuelCreatedIntegrationEvent(int duelId,
                                           IEnumerable<QuestionModel> questions)
        {
            DuelId = duelId;
            Questions = questions;
        }
    }
}
