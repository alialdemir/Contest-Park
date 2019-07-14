using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Post.API.Infrastructure.Tables
{
    [Table("Likes")]
    public class Like : EntityBase
    {
        [Key]
        public int LikeId { get; set; }

        public int PostId { get; set; }

        public string UserId { get; set; }
    }
}
