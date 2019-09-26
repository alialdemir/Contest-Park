using ContestPark.Mobile.Models.DeviceHelper;

namespace ContestPark.Mobile.Dependencies
{
    public interface IDevice
    {
        DeviceHelper GeScreenSize();

        string GetIdentifier();
    }
}
