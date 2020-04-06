using ContestPark.Notification.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Infrastructure.Repositories.PushNotification
{
    public interface IPushNotificationRepository
    {
        string GetTokenByUserId(string userId);

        void RemoveAsync(string userId);

        Task<bool> UpdateTokenByUserIdAsync(PushNotificationTokenModel tokenModel);
    }
}
