using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Enums;
using ContestPark.Post.API.Infrastructure.MySql.Post;
using ContestPark.Post.API.Infrastructure.Repositories.Like;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Models;
using ContestPark.Post.API.Resources;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Post.API.IntegrationEvents.EventHandling
{
    public class PostLikeIntegrationEventHandler : IIntegrationEventHandler<PostLikeIntegrationEvent>

    {
        #region Private variables

        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostLikeIntegrationEventHandler> _logger;
        private readonly IEventBus _eventBus;

        #endregion Private variables

        #region Constructor

        public PostLikeIntegrationEventHandler(ILikeRepository likeRepository,
                                               IPostRepository postRepository,
                                               ILogger<PostLikeIntegrationEventHandler> logger,
                                               IEventBus eventBus)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _logger = logger;
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Post beğen event handler
        /// </summary>
        public async Task Handle(PostLikeIntegrationEvent @event)
        {
            bool isSuccess = await _likeRepository.LikeAsync(@event.UserId, @event.PostId);
            if (!isSuccess)
            {
                _logger.LogError("Post beğenme işlemi başarısız oldu.");

                var @eventErrorMessage = new SendErrorMessageWithSignalrIntegrationEvent(@event.UserId, PostResource.YourLikeProcessDidNotHappenPleaseTryAgain);
                _eventBus.Publish(eventErrorMessage);

                return;
            }

            PostOwnerModel postOwnerModel = _postRepository.GetPostOwnerInfo(@event.PostId);
            if (postOwnerModel == null)
                return;

            // postu beğendiği bildirimi post ile ilişkisi olanlara gönderildi
            var @eventNotification = new AddNotificationIntegrationEvent(@event.UserId,
                                                                         new string[2] { postOwnerModel.OwnerUserId, postOwnerModel.CompetitorUserId },
                                                                         NotificationTypes.PostLike,
                                                                         @event.PostId,
                                                                         postOwnerModel.PicturePath);
            _eventBus.Publish(@eventNotification);
        }

        #endregion Methods
    }
}
