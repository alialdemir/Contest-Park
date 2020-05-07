using Newtonsoft.Json;

namespace ContestPark.Notification.API.Models
{
    public class OneSignalPushNotificationModel
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("filters")]
        public object[] Filters { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("contents")]
        public object Contents { get; set; }

        [JsonProperty("headings")]
        public object Headings { get; set; }

        [JsonProperty("send_after")]
        public string SendAfter { get; set; }
    }
}
