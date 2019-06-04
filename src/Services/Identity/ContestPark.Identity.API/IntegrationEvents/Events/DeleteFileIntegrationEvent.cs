using ContestPark.EventBus.Events;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class DeleteFileIntegrationEvent : IntegrationEvent
    {
        public string Uri { get; private set; }

        public DeleteFileIntegrationEvent(string uri)
        {
            Uri = uri;
        }
    }
}