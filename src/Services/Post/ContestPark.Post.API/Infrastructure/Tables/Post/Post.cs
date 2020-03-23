using ContestPark.Core.Database.Models;
using ContestPark.Post.API.Enums;
using Dapper;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Tables.Post
{
    [Table("Posts")]
    public partial class Post : EntityBase
    {
        [Key]
        public int PostId { get; set; }

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

        public bool IsCommentOpen { get; set; }

        public bool IsArchive { get; set; }

        public string OwnerUserId { get; set; }

        public PostTypes PostType { get; set; }
    }
}
