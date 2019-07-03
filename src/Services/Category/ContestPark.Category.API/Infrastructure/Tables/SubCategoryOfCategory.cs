using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("SubCategoryRls")]// Adı çok uzun olduğu için kısa alt verdim
    public class SubCategoryOfCategory : EntityBase
    {
        [Key]
        public short SubcategoriesOfCategoryId { get; set; }

        public short SubCategoryId { get; set; }
        public short CategoryId { get; set; }
    }
}
