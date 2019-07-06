using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("Categories")]
    public class Category : EntityBase
    {
        [Key]
        public short CategoryId { get; set; }

        public bool Visibility { get; set; }

        public byte DisplayOrder { get; set; }
    }
}
