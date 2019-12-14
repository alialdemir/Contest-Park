using ContestPark.EventBus.Events;

namespace ContestPark.BackgroundTasks.IntegrationEvents
{
    public class DeliverGoldToWinnersIntegrationEvent : IntegrationEvent
    {
        public DeliverGoldToWinnersIntegrationEvent(short contestDateId)
        {
            ContestDateId = contestDateId;
        }

        public short ContestDateId { get; }
    }
}
