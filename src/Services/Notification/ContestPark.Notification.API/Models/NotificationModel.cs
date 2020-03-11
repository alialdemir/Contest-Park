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

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = string.Format(value, WhoUserName); }
        }

        public string ProfilePicturePath { get; set; }

        public bool IsNotificationSeen { get; set; }

        public string Link { get; set; }

        public DateTime Date { get; set; }

        public NotificationTypes NotificationType { get; set; }
    }
}
