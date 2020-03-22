using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.Sms
{
    public interface ISmsService
    {
        Task<bool> SendSms(string message, string phoneNumber);
    }
}
