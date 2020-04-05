using ContestPark.Notification.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure.Repositories.PushNotification
{
    public interface IPushNotificationRepository
    {
        Task<bool> UpdateTokenByUserIdAsync(PushNotificationTokenModel tokenModel);
    }
}
