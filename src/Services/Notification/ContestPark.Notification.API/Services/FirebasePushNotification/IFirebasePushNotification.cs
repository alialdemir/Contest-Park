using ContestPark.Notification.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.FirebasePushNotification
{
    public interface IFirebasePushNotification
    {
        Task<PushNotificationResponseModel> SendPushAsync(PushNotificationModel pushNotification);
    }
}