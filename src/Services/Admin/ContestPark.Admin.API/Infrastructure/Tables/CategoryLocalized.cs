using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using Dapper;

namespace ContestPark.Admin.API.Infrastructure.Tables
{
    [Table("CategoryLocalizeds")]
    public class CategoryLocalized : EntityBase
    {
        [Key]
        public short CategoryLocalizedId { get; set; }

        public short CategoryId { get; set; }

        public string Text { get; set; }

        public Languages Language { get; set; }
    }
}
