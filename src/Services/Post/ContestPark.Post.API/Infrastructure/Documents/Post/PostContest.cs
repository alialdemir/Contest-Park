using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Bet { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorFullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorProfilePicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte CompetitorTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorUserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DuelId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderFullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderProfilePicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte FounderTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderUserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short SubCategoryId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryPicturePath { get; set; }
    }
}