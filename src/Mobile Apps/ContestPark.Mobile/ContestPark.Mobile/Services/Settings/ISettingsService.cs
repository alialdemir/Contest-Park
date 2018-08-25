using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.User;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Settings
{
    public interface ISettingsService
    {
        string AuthAccessToken { get; set; }
        Languages Language { get; set; }
        UserInfoModel UserInfo { get; }

        string SignalRConnectionId { get; set; }

        bool GetValueOrDefault(string key, bool defaultValue);

        string GetValueOrDefault(string key, string defaultValue);

        int GetValueOrDefault(string key, byte defaultValue);

        Task AddOrUpdateValue(string key, bool value);

        Task AddOrUpdateValue(string key, string value);

        Task AddOrUpdateValue(string key, byte value);
    }
}