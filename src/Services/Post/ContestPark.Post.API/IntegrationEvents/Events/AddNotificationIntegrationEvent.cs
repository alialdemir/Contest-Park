using ContestPark.EventBus.Events;
using ContestPark.Post.API.Enums;
using System.Collections.Generic;

namespace ContestPark.Post.API.IntegrationEvents.Events
{
    public class AddNotificationIntegrationEvent : IntegrationEvent
    {
        public AddNotificationIntegrationEvent(string whoId,
                                               IEnumerable<string> whonIds,
                                               NotificationTypes notificationType,
                                               int? postId,
                                               string link)
        {
            WhoId = whoId;
            WhonIds = whonIds;
            NotificationType = notificationType;
            PostId = postId;
            Link = link;
        }

        public string WhoId { get; set; }

        public IEnumerable<string> WhonIds { get; set; }

        public NotificationTypes NotificationType { get; set; }

        public int? PostId { get; set; }
        public string Link { get; set; }
    }
}
