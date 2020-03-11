using ContestPark.Core.Database.Models;
using ContestPark.Notification.API.Enums;
using Dapper;

namespace ContestPark.Notification.API.Infrastructure.Tables
{
    [Table("Notifications")]
    public partial class Notification : EntityBase
    {
        [Key]
        public int NotificationId { get; set; }

        public string WhonId { get; set; }

        public string WhoId { get; set; }

        /// <summary>
        /// Bildirim görülme durumu
        /// </summary>
        public bool IsNotificationSeen { get; set; }

        public NotificationTypes NotificationType { get; set; }

        public int? PostId { get; set; }

        public string Link { get; set; }
    }
}
