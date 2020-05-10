using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using Dapper;

namespace ContestPark.Mission.API.Infrastructure.Tables
{
    [Table("MissionLocalizeds")]
    public class MissionLocalized : EntityBase
    {
        [Key]
        public short MissionLocalizedId { get; set; }

        public byte MissionId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Languages Language { get; set; }
    }
}
