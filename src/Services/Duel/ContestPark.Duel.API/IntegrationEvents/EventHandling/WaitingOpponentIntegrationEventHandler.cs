using ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class WaitingOpponentIntegrationEventHandler : IIntegrationEventHandler<WaitingOpponentIntegrationEvent>
    {
        #region Private variables

        private readonly IDuelUserRepository _duelUserRepository;

        private readonly IEventBus _eventBus;

        private readonly ILogger<WaitingOpponentIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public WaitingOpponentIntegrationEventHandler(IDuelUserRepository duelUserRepository,
                                                      IEventBus eventBus,
                                                      ILogger<WaitingOpponentIntegrationEventHandler> logger)
        {
            _duelUserRepository = duelUserRepository ?? throw new ArgumentNullException(nameof(duelUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        #endregion Constructor

        #region Handle method

        /// <summary>
        /// Kullanıcı rakip arama işlemlerini yapar eşleşen rakip varsa ikisi için düello başlatır yoksa sıraya alır
        /// </summary>
        public Task Handle(WaitingOpponentIntegrationEvent @event)
        {
            if (String.IsNullOrEmpty(@event.UserId) || @event.SubCategoryId < 0 || @event.Bet < 0)
            {
                _logger.LogWarning($@"Duello oluşturulken değerler boş geldi.
                                    {nameof(@event.UserId)}: {@event.UserId}
                                    {nameof(@event.SubCategoryId)}: {@event.SubCategoryId}
                                    {nameof(@event.Bet)}: {@event.Bet}
                                    {nameof(@event.BalanceType)}: {@event.BalanceType}");

                return Task.CompletedTask;
            }

            _logger.LogInformation($@"Rakip bekleniyor...
                                    { nameof(@event.UserId)}: { @event.UserId}
                                    { nameof(@event.SubCategoryId)}: { @event.SubCategoryId}
                                    { nameof(@event.Bet)}: { @event.Bet}
                                    { nameof(@event.BalanceType)}: { @event.BalanceType}");

            DuelUserModel waitingDuelUser = _duelUserRepository.GetDuelUser(new DuelUserModel
            {
                BalanceType = @event.BalanceType,
                Bet = @event.Bet,
                ConnectionId = @event.ConnectionId,
                SubCategoryId = @event.SubCategoryId,
                UserId = @event.UserId
            });

            if (waitingDuelUser != null)// eğer bekleyen rakip varsa onu alır
            {
                StartDuel(@event, waitingDuelUser);
            }
            else// rakip yoksa sıraya alır
            {
                AddAwatingMode(@event);
            }

            return Task.CompletedTask;
        }

        #endregion Handle method

        #region Private methods

        /// <summary>
        /// Kullanıcı redise bekleyenler arasına eklendi.
        /// </summary>
        private void AddAwatingMode(WaitingOpponentIntegrationEvent @event)
        {
            bool isSuccess = _duelUserRepository.Insert(new DuelUserModel
            {
                Bet = @event.Bet,
                ConnectionId = @event.ConnectionId,
                SubCategoryId = @event.SubCategoryId,
                UserId = @event.UserId,
                BalanceType = @event.BalanceType,
                Language = @event.Language
            });

            if (isSuccess)
            {
                _logger.LogInformation($"Rakip bekleyen kullanıcı sıraya alındı. User Id: {@event.UserId}");
            }
            else
            {
                _logger.LogCritical($"CRITICAL: Kullanıcı düello sırasına eklenirken hata oluştu User Id: {@event.UserId}", @event);
            }
        }

        /// <summary>
        /// Singalr ile oyunu başlat
        /// </summary>
        private void StartDuel(WaitingOpponentIntegrationEvent waitingOpponentIntegration, DuelUserModel duelUserModel)
        {
            _duelUserRepository.Delete(duelUserModel);

            var @event = new DuelStartIntegrationEvent(subCategoryId: waitingOpponentIntegration.SubCategoryId,
                                                       bet: waitingOpponentIntegration.Bet,

                                                       founderUserId: waitingOpponentIntegration.UserId,
                                                       founderConnectionId: waitingOpponentIntegration.ConnectionId,
                                                       founderLanguage: waitingOpponentIntegration.Language,

                                                       opponentUserId: duelUserModel.UserId,
                                                       opponentConnectionId: duelUserModel.ConnectionId,
                                                       opponentLanguage: duelUserModel.Language,

                                                       balanceType: duelUserModel.BalanceType);
            _eventBus.Publish(@event);

            _logger.LogInformation("Rakipler eşleşti.", new
            {
                FounderUserId = waitingOpponentIntegration.UserId,
                OpponentUserId = duelUserModel.UserId
            });
        }

        #endregion Private methods
    }
}
