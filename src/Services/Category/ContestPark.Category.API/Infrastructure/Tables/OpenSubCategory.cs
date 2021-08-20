using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("OpenSubCategories")]
    public class OpenSubCategory : EntityBaseEffaceable
    {
        [Key]
        public int OpenSubCategoryId { get; set; }

        public string UserId { get; set; }

        public short SubCategoryId { get; set; }
    }
}
