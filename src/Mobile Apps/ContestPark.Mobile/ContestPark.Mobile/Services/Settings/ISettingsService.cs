using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.User;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Settings
{
    public interface ISettingsService
    {
        string AuthAccessToken { get; set; }
        Languages Language { get; set; }
        UserInfoModel CurrentUser { get; }

        bool IsPrivatePrice { get; }

        bool IsSoundEffectActive { get; }

        string SignalRConnectionId { get; set; }

        bool GetValueOrDefault(bool defaultValue, [CallerMemberName]string methodName = "");

        string GetValueOrDefault(string defaultValue, [CallerMemberName]string methodName = "");

        int GetValueOrDefault(byte defaultValue, [CallerMemberName]string methodName = "");

        Task AddOrUpdateValue(bool value, [CallerMemberName]string methodName = "");

        Task AddOrUpdateValue(string value, [CallerMemberName]string methodName = "");

        Task AddOrUpdateValue(byte value, [CallerMemberName]string methodName = "");

        void RemoveCurrentUser();

        #region Api

        Task SetSettingsAsync(SettingTypes settingType, string settingValue);

        #endregion Api
    }
}