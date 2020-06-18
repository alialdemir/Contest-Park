using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("LevelUps")]
    public class LevelUp : EntityBase
    {
        [Key]
        public short Level { get; set; }

        public int Exp { get; set; }
    }
}
