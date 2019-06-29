using ContestPark.Core.CosmosDb.Models;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post : DocumentBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Bet { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? CompetitorTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DuelId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? FounderTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryId { get; set; }
    }
}