using ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Resources;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class RemoveWaitingOpponentIntegrationEventHandler : IIntegrationEventHandler<RemoveWaitingOpponentIntegrationEvent>
    {
        #region Private variables

        private readonly IDuelUserRepository _duelUserRepository;

        private readonly IEventBus _eventBus;

        private readonly ILogger<RemoveWaitingOpponentIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public RemoveWaitingOpponentIntegrationEventHandler(IDuelUserRepository duelUserRepository,
                                                            IEventBus eventBus,
                                                            ILogger<RemoveWaitingOpponentIntegrationEventHandler> logger)
        {
            _duelUserRepository = duelUserRepository ?? throw new ArgumentNullException(nameof(duelUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Rakip bekleme modundan çıkar
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(RemoveWaitingOpponentIntegrationEvent @event)
        {
            if (string.IsNullOrEmpty(@event.UserId) || @event.SubCategoryId < 0 || @event.Bet < 0)
            {
                _logger.LogWarning("Duello rakip bekleme modundan çıkarırken değerler boş geldi. {UserId} {SubCategoryId} {Bet} {BalanceType}",
                                   @event.UserId,
                                   @event.SubCategoryId,
                                   @event.Bet,
                                   @event.BalanceType);

                return;
            }

            try
            {
                _logger.LogInformation("Rakip bekleme modundan çıktı. {UserId} {SubCategoryId} {Bet} {BalanceType}",
                                       @event.UserId,
                                       @event.SubCategoryId,
                                       @event.Bet,
                                       @event.BalanceType);

                bool isSuccess = await Task.Factory.StartNew(() => _duelUserRepository.Delete(new DuelUserModel
                {
                    BalanceType = @event.BalanceType,
                    Bet = @event.Bet,
                    ConnectionId = @event.ConnectionId,
                    SubCategoryId = @event.SubCategoryId,
                    UserId = @event.UserId
                }));

                if (!isSuccess)
                {
                    _logger.LogCritical("CRITICAL: Düello rakip bekleme modundan çıkarılırken hata oluştu. {UserId} {SubCategoryId} {Bet} {BalanceType}",
                                       @event.UserId,
                                       @event.SubCategoryId,
                                       @event.Bet,
                                       @event.BalanceType);

                    SendErrorMessage(@event.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRITICAL: Düello rakip bekleme modundan çıkarılırken exception oluştu.. {UserId} {SubCategoryId} {Bet} {BalanceType}",
                                   @event.UserId,
                                   @event.SubCategoryId,
                                   @event.Bet,
                                   @event.BalanceType);

                SendErrorMessage(@event.UserId);
            }
        }

        private void SendErrorMessage(string userId)
        {
            var @event = new SendErrorMessageWithSignalrIntegrationEvent(userId, DuelResource.AnErrorOccurredDuringTheProcessOfExitingTheStandbyMode);

            _eventBus.Publish(@event);
        }

        #endregion Methods
    }
}
