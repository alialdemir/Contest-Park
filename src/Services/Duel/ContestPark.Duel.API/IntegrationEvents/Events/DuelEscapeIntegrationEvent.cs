using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
{
    public class DuelEscapeIntegrationEvent : IntegrationEvent
    {
        public int DuelId { get; set; }
        public string EscaperUserId { get; set; }
        public bool IsDuelCancel { get; set; }

        public DuelEscapeIntegrationEvent(int duelId,
                                          string escaperUserId,
                                          bool isDuelCancel)
        {
            DuelId = duelId;
            EscaperUserId = escaperUserId;
            IsDuelCancel = isDuelCancel;
        }
    }
}
