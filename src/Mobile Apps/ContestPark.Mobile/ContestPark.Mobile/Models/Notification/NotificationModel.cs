using ContestPark.Mobile.Enums;
using System;

namespace ContestPark.Mobile.Models.Notification
{
    public class NotificationModel : BaseModel
    {
        public int NotificationId { get; set; }

        public string WhoUserId { get; set; }

        public string WhoFullName { get; set; }

        public string WhoUserName { get; set; }

        public string Description { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool IsNotificationSeen { get; set; }

        public string Link { get; set; }

        public DateTime Date { get; set; }

        public NotificationTypes NotificationType { get; set; }
    }
}
