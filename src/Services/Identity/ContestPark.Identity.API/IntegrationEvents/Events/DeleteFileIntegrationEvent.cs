using ContestPark.EventBus.Events;
using ContestPark.Identity.API.Enums;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class DeleteFileIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; }
        public string Uri { get; private set; }
        public PictureTypes PictureType { get; }

        public DeleteFileIntegrationEvent(string userId, string uri, PictureTypes pictureType)
        {
            UserId = userId;
            Uri = uri;
            PictureType = pictureType;
        }
    }
}
