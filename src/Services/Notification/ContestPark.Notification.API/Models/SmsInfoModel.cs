using Newtonsoft.Json;

namespace ContestPark.Notification.API.Models
{
    public class SmsInfoModel
    {
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }

        [JsonIgnore]
        public string PhoneNumberWithCountryCode
        {
            get { return $"{CountryCode}{PhoneNumber}"; }
        }
    }
}
