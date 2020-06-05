using ContestPark.Identity.API.Enums;

namespace ContestPark.Identity.API.Data.Tables
{
    public class DeviceInfo
    {
        public int DeviceInfoId { get; set; }
        public string UserId { get; set; }
        public string DeviceIdentifier { get; set; }
        public Platforms Platform { get; set; }
        public NetworkAccess NetworkAccess { get; set; }
    }
}
