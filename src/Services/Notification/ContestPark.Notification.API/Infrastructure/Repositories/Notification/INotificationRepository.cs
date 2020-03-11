using ContestPark.Notification.API.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure.Repositories.Notification
{
    public interface INotificationRepository
    {
        Task<bool> AddRangeAsync(IEnumerable<Tables.Notification> notifications);

        bool IsNotificationBeAdded(NotificationTypes notificationType, int postId, string whoId, string link);
    }
}
