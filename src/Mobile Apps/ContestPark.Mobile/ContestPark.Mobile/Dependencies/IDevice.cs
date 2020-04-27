using ContestPark.Mobile.Models.DeviceHelper;

namespace ContestPark.Mobile.Dependencies
{
    public interface IDevice
    {
        string GetIdentifier();

        void DismissKeyboard();

        void CloseApp();
    }
}
