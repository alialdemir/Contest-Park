using Newtonsoft.Json;

namespace ContestPark.Core.Models
{
    public class UserModel
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string ProfilePicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CoverPicturePath { get; set; }

        public string UserId { get; set; }
    }
}
