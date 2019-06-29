using ContestPark.Post.API.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ContestPark.Post.API.Models.Post
{
    public partial class PostModel
    {
        public DateTime Date { get; set; }

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        public bool IsLike { get; set; }

        public string OwnerFullName { get; set; }
        public string OwnerProfilePicturePath { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerUserId { get; set; }
        public string PostId { get; set; }

        public PostTypes PostType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> UserIds { get; set; }
    }
}
