using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class DuelEscapeIntegrationEvent : IntegrationEvent
    {
        public int DuelId { get; set; }
        public string EscaperUserId { get; }
        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }
        public List<DuelFinishQuestionModel> Questions { get; set; }

        public DuelEscapeIntegrationEvent(int duelId,
                                          string escaperUserId,
                                          string founderUserId,
                                          string opponentUserId,
                                          List<DuelFinishQuestionModel> questions)
        {
            DuelId = duelId;
            EscaperUserId = escaperUserId;
            FounderUserId = founderUserId;
            OpponentUserId = opponentUserId;
            Questions = questions;
        }
    }
}
