using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Repositories.Post;
using ContestPark.Post.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Post.API.IntegrationEvents.EventHandling
{
    public class NewPostAddedIntegrationEventHandler : IIntegrationEventHandler<NewPostAddedIntegrationEvent>

    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<NewPostAddedIntegrationEventHandler> _logger;

        public NewPostAddedIntegrationEventHandler(IPostRepository postRepository,
                                                   ILogger<NewPostAddedIntegrationEventHandler> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        /// <summary>
        /// Post ekle
        /// </summary>
        /// <param name="event">Eklenen post detayı</param>
        public async Task Handle(NewPostAddedIntegrationEvent @event)
        {
            bool isSuccess = await _postRepository.AddPost(new Infrastructure.Documents.Post
            {
                Bet = @event.Bet,
                CompetitorTrueAnswerCount = @event.CompetitorTrueAnswerCount,
                CompetitorUserId = @event.CompetitorUserId,
                Description = @event.Description,
                DuelId = @event.DuelId,
                FounderTrueAnswerCount = @event.FounderTrueAnswerCount,
                FounderUserId = @event.FounderUserId,
                OwnerUserId = @event.OwnerUserId,
                PicturePath = @event.PicturePath,
                PostImageType = @event.PostImageType,
                SubCategoryId = @event.SubCategoryId,
                PostType = @event.PostType,
            });

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: post eklenemedi post bilgileri. Lütfen manuel ekleyin..", @event.Bet,
                    @event.CompetitorTrueAnswerCount,
                    @event.CompetitorUserId,
                    @event.Description,
                    @event.DuelId,
                    @event.FounderTrueAnswerCount,
                    @event.FounderUserId,
                    @event.OwnerUserId,
                    @event.PicturePath,
                    @event.PostImageType,
                    @event.SubCategoryId,
                    @event.PostType);
            }
        }
    }
}
