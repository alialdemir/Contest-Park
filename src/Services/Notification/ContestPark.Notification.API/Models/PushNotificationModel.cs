using Newtonsoft.Json;

namespace ContestPark.Notification.API.Models
{
    public class PushNotificationModel
    {
        /// <summary>
        /// Device id veya topics
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// Bildirimde gönderilecek veri
        /// </summary>
        [JsonProperty("notification")]
        public PushNotificationMessageModel Notification { get; set; }
    }

    public class PushNotificationMessageModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
