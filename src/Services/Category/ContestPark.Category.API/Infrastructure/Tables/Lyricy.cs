using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("Lyrics")]
    public class Lyricy :EntityBase
    {
        [Key]
        public int LyricyId { get; set; }

        public int SongId { get; set; }

        public int Duration { get; set; }

        public string Text { get; set; }
    }
}
