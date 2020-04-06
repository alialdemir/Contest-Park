using ContestPark.Core.Enums;
using ContestPark.EventBus.Events;
using ContestPark.Notification.API.Enums;

namespace ContestPark.Notification.API.IntegrationEvents.Events
{
    public class SendPushNotificationIntegrationEvent : IntegrationEvent
    {
        public SendPushNotificationIntegrationEvent(PushNotificationTypes pushNotificationType,
                                                    Languages currentUserLanguage,
                                                    string userId)
        {
            PushNotificationType = pushNotificationType;
            CurrentUserLanguage = currentUserLanguage;
            UserId = userId;
        }

        public PushNotificationTypes PushNotificationType { get; set; }
        public Languages CurrentUserLanguage { get; set; }
        public string UserId { get; set; }
    }
}
