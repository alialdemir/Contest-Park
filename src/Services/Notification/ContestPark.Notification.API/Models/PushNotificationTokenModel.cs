using Newtonsoft.Json;

namespace ContestPark.Notification.API.Models
{
    public class PushNotificationTokenModel
    {
        public string Token { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
    }
}
