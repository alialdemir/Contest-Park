using ContestPark.Core.CosmosDb.Models;
using ContestPark.Post.API.Enums;
using System;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post : DocumentBase
    {
        public int CommentCount { get; set; }

        public DateTime Date { get; set; }

        public bool IsLike { get; set; }

        public int LikeCount { get; set; }

        public string OwnerFullName { get; set; }
        public string OwnerProfilePicturePath { get; set; }
        public string OwnerUserName { get; set; }
        public string PostId { get; set; }

        public PostTypes PostType { get; set; }
    }
}