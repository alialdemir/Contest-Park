using System;

namespace ContestPark.Notification.API.Models
{
    public class PushNotificationMessageModel
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string Icon { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string UserId { get; set; }
    }
}
