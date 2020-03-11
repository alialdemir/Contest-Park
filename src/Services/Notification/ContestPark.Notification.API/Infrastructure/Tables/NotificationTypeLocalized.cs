using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Notification.API.Enums;
using Dapper;

namespace ContestPark.Notification.API.Infrastructure.Tables
{
    [Table("NotificationTypeLocalizeds")]
    public class NotificationTypeLocalized : EntityBase
    {
        [Key]
        public short NotificationLocalizedId { get; set; }

        public string Description { get; set; }

        public Languages Language { get; set; }

        public NotificationTypes NotificationType { get; set; }
    }
}
