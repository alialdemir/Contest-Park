using ContestPark.Mission.API.Enums;

namespace ContestPark.Mission.API.Models
{
    public class MissionModel
    {
        public byte MissionId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Reward { get; set; }

        public BalanceTypes RewardBalanceType { get; set; }

        public string PicturePath { get; set; }

        public bool IsCompleteMission { get; set; }

        public MissionTime MissionTime { get; set; }
    }
}
