using ContestPark.Core.Database.Models;

namespace ContestPark.Mission.API.Models
{
    public class MissionServiceModel : ServiceModel<MissionModel>
    {
        public byte CompletedMissionCount { get; set; }
    }
}
