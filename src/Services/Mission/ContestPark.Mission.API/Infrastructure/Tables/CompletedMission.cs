using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Mission.API.Infrastructure.Tables
{
    [Table("CompletedMissions")]
    public class CompletedMission : EntityBase
    {
        [Key]
        public int CompletedMissionId { get; set; }

        public byte MissionId { get; set; }

        public string UserId { get; set; }

        public bool MissionComplate { get; set; }
    }
}
