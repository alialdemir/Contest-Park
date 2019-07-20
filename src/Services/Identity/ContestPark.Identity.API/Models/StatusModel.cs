using Newtonsoft.Json;

namespace ContestPark.Identity.API.Models
{
    public class StatusModel
    {
        [JsonProperty("IsBlocked")]
        public bool IsStatus { get; set; }
    }
}
