using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("SubCategoryLocalizeds")]
    public class SubCategoryLocalized : EntityBase
    {
        [Key]
        public short SubCategoryLocalizedId { get; set; }

        public short SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }
        public string Description { get; set; }
        public Languages Language { get; set; }
    }
}
