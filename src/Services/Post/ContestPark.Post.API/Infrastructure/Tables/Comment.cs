using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Post.API.Infrastructure.Tables
{
    [Table("Comments")]
    public class Comment : EntityBase
    {
        [Key]
        public int CommentId { get; set; }

        public string Text { get; set; }

        public string UserId { get; set; }

        public int PostId { get; set; }
    }
}
