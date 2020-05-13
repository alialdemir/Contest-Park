using ContestPark.EventBus.Events;

namespace ContestPark.Mission.API.IntegrationEvents.Events
{
    public class CheckMissionIntegrationEvent : IntegrationEvent
    {
        public CheckMissionIntegrationEvent(byte missionId,
                                            string userId)
        {
            MissionId = missionId;
            UserId = userId;
        }

        public byte MissionId { get; set; }
        public string UserId { get; set; }
    }
}
