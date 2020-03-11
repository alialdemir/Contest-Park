using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Notification.API.Enums;
using ContestPark.Notification.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure.Repositories.Notification
{
    public interface INotificationRepository
    {
        Task<bool> AddRangeAsync(IEnumerable<Tables.Notification> notifications);

        bool IsNotificationBeAdded(NotificationTypes notificationType, int postId, string whoId, string link);
        ServiceModel<NotificationModel> Notifications(string userId, Languages langId, PagingModel pagingModel);
    }
}
