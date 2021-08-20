using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("Lyrics")]
    public class Lyrics :EntityBase
    {
        [Key]
        public int LyricsId { get; set; }

        public short SongId { get; set; }

        public int Duration { get; set; }

        public string Text { get; set; }
    }
}
