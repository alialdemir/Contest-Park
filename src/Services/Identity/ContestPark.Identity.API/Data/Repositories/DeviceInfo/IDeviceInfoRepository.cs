namespace ContestPark.Identity.API.Data.Repositories.DeviceInfo
{
    public interface IDeviceInfoRepository
    {
        bool CheckDeviceIdentifier(string deviceIdentifier);
        bool Insert(string deviceIdentifier);
    }
}
