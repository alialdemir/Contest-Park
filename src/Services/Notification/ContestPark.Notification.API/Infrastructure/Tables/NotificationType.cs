using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Notification.API.Infrastructure.Tables
{
    [Table("NotificationTypes")]
    public class NotificationType : EntityBase
    {
        [Key]
        public byte NotificationTypeId { get; set; }

        public bool IsActive { get; set; }
    }
}
