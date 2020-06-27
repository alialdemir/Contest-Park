using ContestPark.Category.API.Infrastructure.Repositories.UserLevel;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Category.API.IntegrationEvents.EventHandling
{
    public class UpdateLevelIntegrationEventHandler : IIntegrationEventHandler<UpdateLevelIntegrationEvent>
    {
        #region Private variables

        private readonly ILogger<UpdateLevelIntegrationEventHandler> _logger;
        private readonly IUserLevelRepository _userLevelRepository;

        #endregion Private variables

        #region Construcotr

        public UpdateLevelIntegrationEventHandler(ILogger<UpdateLevelIntegrationEventHandler> logger,
                                                  IUserLevelRepository userLevelRepository)
        {
            _logger = logger;
            _userLevelRepository = userLevelRepository;
        }

        #endregion Construcotr

        #region Methods

        /// <summary>
        /// Oyuncunun seviyesine parametreden gelen exp değeri ile toplar eğer level atlamışsa leveli atlatır
        /// </summary>
        public async Task Handle(UpdateLevelIntegrationEvent @event)
        {
            if (!@event.FounderUserId.EndsWith("-bot"))
            {
                bool isSuccess = await _userLevelRepository.UpdateLevel(@event.FounderUserId, @event.SubCategoryId, @event.FounderExp);
                if (!isSuccess)
                {
                    _logger.LogError("Level güncelleme işlemi gerçekleşemedi, userId: {userId}, subCategoryId: {subCategoryId} exp: {exp}",
                                     @event.FounderUserId,
                                     @event.SubCategoryId,
                                     @event.FounderExp);
                }
            }

            if (!@event.OpponentUserId.EndsWith("-bot"))
            {
                bool isSuccess = await _userLevelRepository.UpdateLevel(@event.OpponentUserId, @event.SubCategoryId, @event.OpponentExp);
                if (!isSuccess)
                {
                    _logger.LogError("Level güncelleme işlemi gerçekleşemedi, userId: {userId}, subCategoryId: {subCategoryId} exp: {exp}",
                                     @event.OpponentUserId,
                                     @event.SubCategoryId,
                                     @event.OpponentExp);
                }
            }
        }

        #endregion Methods
    }
}
