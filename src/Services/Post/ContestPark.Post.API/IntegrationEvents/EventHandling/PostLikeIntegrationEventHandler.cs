using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Resources;
using System.Threading.Tasks;

namespace ContestPark.Post.API.IntegrationEvents.EventHandling
{
    public class PostLikeIntegrationEventHandler : IIntegrationEventHandler<PostLikeIntegrationEvent>

    {
        private readonly ILikeRepository _likeRepository;
        private readonly IEventBus _eventBus;

        public PostLikeIntegrationEventHandler(ILikeRepository likeRepository,
                                               IEventBus eventBus)
        {
            _likeRepository = likeRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Post beğen event handler
        /// </summary>
        public async Task Handle(PostLikeIntegrationEvent @event)
        {
            bool isSuccess = await _likeRepository.LikeAsync(@event.UserId, @event.PostId);
            if (!isSuccess)
            {
                var @eventErrorMessage = new SendErrorMessageWithSignalrIntegrationEvent(@event.UserId, PostResource.YourLikeProcessDidNotHappenPleaseTryAgain);
                _eventBus.Publish(eventErrorMessage);
            }
        }
    }
}
