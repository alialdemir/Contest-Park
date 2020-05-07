using ContestPark.Balance.API.Enums;
using ContestPark.Core.Enums;
using ContestPark.EventBus.Events;
using System;

namespace ContestPark.Balance.API.IntegrationEvents.Events
{
    public class SendPushNotificationIntegrationEvent : IntegrationEvent
    {
        public SendPushNotificationIntegrationEvent(PushNotificationTypes pushNotificationType,
                                                    Languages currentUserLanguage,
                                                    string userId,
                                                    DateTime? scheduleDate)
        {
            PushNotificationType = pushNotificationType;
            CurrentUserLanguage = currentUserLanguage;
            UserId = userId;
            ScheduleDate = scheduleDate;
        }

        public PushNotificationTypes PushNotificationType { get; set; }
        public Languages CurrentUserLanguage { get; set; }
        public string UserId { get; set; }
        public DateTime? ScheduleDate { get; set; }
    }
}
