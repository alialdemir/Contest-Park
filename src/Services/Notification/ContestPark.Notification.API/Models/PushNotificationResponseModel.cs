using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Notification.API.Models
{
    public class PushNotificationResponseModel
    {
        [JsonProperty("multicast_id")]
        public long MulticastId { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("failure")]
        public bool Failure { get; set; }

        [JsonProperty("canonical_ids")]
        public int CanonicalIds { get; set; }

        [JsonProperty("results")]
        public List<PushNotificationErrorModel> Results { get; set; }
    }

    public class PushNotificationErrorModel
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
