using Newtonsoft.Json;

namespace ContestPark.Duel.API.Models
{
    public class RankModel
    {
        public string TotalScore { get; set; }

        public string UserFullName { get; set; }

        public string UserProfilePicturePath { get; set; }

        public string UserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }
    }
}
