using ContestPark.Mobile.Enums;
using System;

namespace ContestPark.Mobile.Models.Notification
{
    public class NotificationModel : BaseModel
    {
        public int NotificationId { get; set; }

        public string PicturePath { get; set; }

        public string WhoFullName { get; set; }

        public string WhoUserName { get; set; }

        public DateTime Date { get; set; }

        public NotificationTypes NotificationType { get; set; }

        public string Text { get; set; }
    }
}