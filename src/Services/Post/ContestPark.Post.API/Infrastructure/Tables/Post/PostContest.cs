using ContestPark.Post.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Tables.Post
{
    public partial class Post
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Bet { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? CompetitorTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DuelId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? FounderTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? SubCategoryId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BalanceTypes BalanceType { get; set; }
    }
}
