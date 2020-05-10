using ContestPark.Core.Database.Models;
using ContestPark.Mission.API.Enums;
using Dapper;

namespace ContestPark.Mission.API.Infrastructure.Tables
{
    [Table("Missions")]
    public class Mission : EntityBase
    {
        [Key]
        public byte MissionId { get; set; }

        public decimal Reward { get; set; }

        public BalanceTypes RewardBalanceType { get; set; }

        public MissionTime MissionTime { get; set; }

        public string PicturePath { get; set; }

        public bool Visibility { get; set; }
    }
}
