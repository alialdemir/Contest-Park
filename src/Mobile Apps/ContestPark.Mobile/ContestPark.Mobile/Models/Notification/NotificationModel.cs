using ContestPark.Mobile.Enums;
using System;

namespace ContestPark.Mobile.Models.Notification
{
    public class NotificationModel : BaseModel
    {
        public int NotificationId { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool IsNotificationSeen { get; set; }

        public string Link { get; set; }

        public DateTime Date { get; set; }

        public int PostId { get; set; }

        public NotificationTypes NotificationType { get; set; }
        private bool _isFollowing;

        public bool IsFollowing
        {
            get { return _isFollowing; }
            set
            {
                _isFollowing = value;
                RaisePropertyChanged(() => IsFollowing);
            }
        }
    }
}
