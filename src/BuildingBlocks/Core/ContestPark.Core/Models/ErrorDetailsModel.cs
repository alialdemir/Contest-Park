using Newtonsoft.Json;

namespace ContestPark.Core.Models
{
    public class ErrorDetailsModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("developerMessage", NullValueHandling = NullValueHandling.Ignore)]
        public object DeveloperMessage { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}