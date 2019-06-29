using ContestPark.EventBus.Events;

namespace ContestPark.Post.API.IntegrationEvents.Events
{
    public class PostLikeIntegrationEvent : IntegrationEvent
    {
        public PostLikeIntegrationEvent(string userId, string postId)
        {
            UserId = userId;
            PostId = postId;
        }

        public string UserId { get; }
        public string PostId { get; }
    }
}
