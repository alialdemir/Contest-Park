using ContestPark.Mobile.Models.ServiceModel;

namespace ContestPark.Mobile.Models.Mission
{
    public class MissionServiceModel : ServiceModel<MissionModel>
    {
        public byte CompletedMissionCount { get; set; }
    }
}
