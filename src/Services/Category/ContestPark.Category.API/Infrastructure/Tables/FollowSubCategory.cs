using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("FollowSubCategories")]
    public class FollowSubCategory : EntityBase
    {
        [Key]
        public int FollowSubCategoryId { get; set; }

        public short SubCategoryId { get; set; }

        public string UserId { get; set; }
    }
}
