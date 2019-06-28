using ContestPark.Core.CosmosDb.Models;
using ContestPark.Post.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post : DocumentBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CommentCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? LikeCount { get; set; }

        public string OwnerUserId { get; set; }

        public PostTypes PostType { get; set; }
    }
}