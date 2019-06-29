using ContestPark.Core.CosmosDb.Models;
using ContestPark.Post.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post : DocumentBase
    {
        private int? commentCount;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CommentCount
        {
            get { return commentCount; }
            set
            {
                commentCount = value > 0 ? value : null;
            }
        }

        private int? likeCount;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? LikeCount
        {
            get { return likeCount; }
            set
            {
                likeCount = value > 0 ? value : null;
            }
        }

        public string OwnerUserId { get; set; }

        public PostTypes PostType { get; set; }
    }
}
