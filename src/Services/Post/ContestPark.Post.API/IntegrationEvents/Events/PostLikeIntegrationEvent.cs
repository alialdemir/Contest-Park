using ContestPark.EventBus.Events;

namespace ContestPark.Post.API.IntegrationEvents.Events
{
    public class PostLikeIntegrationEvent : IntegrationEvent
    {
        public PostLikeIntegrationEvent(string userId, int postId)
        {
            UserId = userId;
            PostId = postId;
        }

        public string UserId { get; }
        public int PostId { get; }
    }
}
