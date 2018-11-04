using ContestPark.Mobile.Models.ServiceModel;

namespace ContestPark.Mobile.Models.Mission
{
    public class MissionListModel : ServiceModel<MissionModel>
    {
        public byte CompleteMissionCount { get; set; }
    }
}