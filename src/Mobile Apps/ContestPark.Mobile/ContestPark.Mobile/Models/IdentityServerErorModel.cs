using Newtonsoft.Json;

namespace ContestPark.Mobile.Models
{
    public class IdentityServerErorModel
    {
        [JsonProperty("error")]
        public string error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}