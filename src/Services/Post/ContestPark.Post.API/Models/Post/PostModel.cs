using ContestPark.Post.API.Enums;
using Newtonsoft.Json;
using System;

namespace ContestPark.Post.API.Models.Post
{
    public partial class PostModel
    {
        public DateTime Date { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? LikeCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CommentCount { get; set; }

        public bool IsLike { get; set; }

        public string OwnerFullName { get; set; }
        public string OwnerProfilePicturePath { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerUserId { get; set; }
        public int PostId { get; set; }

        public PostTypes PostType { get; set; }
    }
}
