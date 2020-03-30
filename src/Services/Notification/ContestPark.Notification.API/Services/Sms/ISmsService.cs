using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.Sms
{
    public interface ISmsService
    {
        bool Delete(string userId);
        int GetSmsCode(string userId);
        bool Insert(string userId, int code);
        Task<bool> SendSms(string message, string phoneNumber);
    }
}
