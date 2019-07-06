using ContestPark.EventBus.Events;

namespace ContestPark.Follow.API.IntegrationEvents.Events
{
    public class UnFollowIntegrationEvent : IntegrationEvent
    {
        public UnFollowIntegrationEvent(string followUpUserId, string followedUserId)
        {
            FollowUpUserId = followUpUserId;
            FollowedUserId = followedUserId;
        }

        /// <summary>
        /// Takip eden
        /// </summary>
        public string FollowUpUserId { get; set; }

        /// <summary>
        /// Takip edilen
        /// </summary>
        public string FollowedUserId { get; set; }
    }
}
