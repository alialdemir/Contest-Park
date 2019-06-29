using ContestPark.EventBus.Events;

namespace ContestPark.Post.API.IntegrationEvents.Events
{
    public class PostUnLikeIntegrationEvent : IntegrationEvent
    {
        public PostUnLikeIntegrationEvent(string userId, string postId)
        {
            UserId = userId;
            PostId = postId;
        }

        public string UserId { get; }
        public string PostId { get; }
    }
}
