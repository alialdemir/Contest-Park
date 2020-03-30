using ContestPark.Mobile.Models.Notification;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Notification
{
    public interface INotificationService
    {
        Task<bool> CheckSmsCode(SmsModel smsModel);
        Task<bool> LogInSms(SmsInfoModel smsInfo);

        Task<ServiceModel<NotificationModel>> NotificationsAsync(PagingModel pagingModel);
    }
}
