using System.Collections.Generic;
using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("Artists")]
    public class Artist : EntityBase
    {

        [Key]
        public short ArtistId { get; set; }

        public string ArtistName { get; set; }

        public string PicturePath { get; set; }

        public string CoverPicturePath { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public short CategoryId { get; set; }

        public List<Song> Songs { get; set; }
    }
}
