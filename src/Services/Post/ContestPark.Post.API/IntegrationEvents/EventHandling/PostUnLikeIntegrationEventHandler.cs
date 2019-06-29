using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Resources;
using System.Threading.Tasks;

namespace ContestPark.Post.API.IntegrationEvents.EventHandling
{
    public class PostUnLikeIntegrationEventHandler : IIntegrationEventHandler<PostUnLikeIntegrationEvent>

    {
        private readonly ILikeRepository _likeRepository;
        private readonly IEventBus _eventBus;

        public PostUnLikeIntegrationEventHandler(ILikeRepository likeRepository,
                                                 IEventBus eventBus)
        {
            _likeRepository = likeRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Postu beğenmekten vazgeç eventhandler
        /// </summary>
        public async Task Handle(PostUnLikeIntegrationEvent @event)
        {
            bool isSuccess = await _likeRepository.UnLikeAsync(@event.UserId, @event.PostId);
            if (!isSuccess)
            {
                var @eventErrorMessage = new SendErrorMessageWithSignalrIntegrationEvent(@event.UserId, PostResource.AnUnexpectedErrorOccurredDuringTheRatingRemoval);
                _eventBus.Publish(eventErrorMessage);
            }
        }
    }
}
