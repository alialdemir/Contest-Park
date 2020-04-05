using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Notification.API.Infrastructure.Tables
{
    [Table("PushNotifications")]
    public class PushNotification : EntityBase
    {
        [Key]
        public string UserId { get; set; }

        public string Token { get; set; }
    }
}
