using ContestPark.Identity.API.Enums;

namespace ContestPark.Identity.API.Data.Repositories.DeviceInfo
{
    public interface IDeviceInfoRepository
    {
        bool CheckDeviceIdentifier(string deviceIdentifier);

        bool Insert(string userId, string deviceIdentifier, Platforms platform, NetworkAccess connectionProfile);
    }
}
