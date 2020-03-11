using ContestPark.Notification.API.Enums;
using System;

namespace ContestPark.Notification.API.Models
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }

        public string WhoUserId { get; set; }

        public string WhoFullName { get; set; }

        public string WhoUserName { get; set; }

        public string Description { get; set; }

        public string PicturePath { get; set; }

        public bool NotificationStatus { get; set; }

        public string Link { get; set; }

        public DateTime Date { get; set; }

        public NotificationTypes NotificationType { get; set; }
    }
}
