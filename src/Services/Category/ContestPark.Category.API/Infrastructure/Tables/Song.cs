using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("Songs")]
    public class Song : EntityBase
    {
        [Key]
        public int SongId { get; set; }

        public string SongName { get; set; }

        public int ArtistId { get; set; }

        public string ExternalId { get; set; }

        public string Language { get; set; }

        public string Language_cc { get; set; }

        public string FullLanguage { get; set; }

        public string Duration { get; set; }

        public string Lyrics { get; set; }

        public string Slug { get; set; }

        public string YoutubeVideoId { get; set; }

        public string PicturePath { get; set; }

        public string Provider { get; set; }
    }
}