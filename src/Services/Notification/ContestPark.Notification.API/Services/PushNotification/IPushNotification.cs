using ContestPark.Notification.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.PushNotification
{
    public interface IPushNotification
    {
        Task<bool> SendPushAsync(PushNotificationMessageModel pushNotification);
    }
}
