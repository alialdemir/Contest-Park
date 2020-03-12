using ContestPark.Notification.API.Enums;
using System;

namespace ContestPark.Notification.API.Models
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool IsNotificationSeen { get; set; }

        public string Link { get; set; }
        
        public int PostId { get; set; }

        public DateTime Date { get; set; }

        public NotificationTypes NotificationType { get; set; }

        public bool? IsFollowing { get; set; }
    }
}
