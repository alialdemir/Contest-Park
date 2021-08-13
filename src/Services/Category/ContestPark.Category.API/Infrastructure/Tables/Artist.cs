using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("Artists")]
    public class Artist : EntityBase
    {

        [Key]
        public int ArtistId { get; set; }

        public string ArtistName { get; set; }

        public string PicturePath { get; set; }

        public string CoverPicturePath { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }
    }
}
