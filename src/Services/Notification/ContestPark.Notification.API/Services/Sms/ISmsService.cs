using ContestPark.Notification.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.Sms
{
    public interface ISmsService
    {
        bool Delete(string key);

        SmsRedisModel GetSmsCode(string key);

        bool Insert(SmsRedisModel sms);

        Task<bool> SendSms(string message, string phoneNumber);
    }
}
