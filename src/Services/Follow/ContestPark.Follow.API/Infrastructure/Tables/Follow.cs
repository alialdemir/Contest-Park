using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Follow.API.Infrastructure.Tables
{
    [Table("Follows")]
    public class Follow : EntityBase
    {
        [Key]
        public int FollowId { get; set; }

        public string FollowUpUserId { get; set; }//Takip eden
        public string FollowedUserId { get; set; }//Takip edilen
    }
}
