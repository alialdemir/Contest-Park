using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("UserLevels")]
    public class UserLevel : EntityBase
    {
        [Key]
        public int UserLevelId { get; set; }

        public string UserId { get; set; }

        public short Level { get; set; }

        public int Exp { get; set; }

        public short SubCategoryId { get; set; }
    }
}
