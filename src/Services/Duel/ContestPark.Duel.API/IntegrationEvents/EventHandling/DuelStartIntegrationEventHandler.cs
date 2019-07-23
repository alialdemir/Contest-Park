using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class DuelStartIntegrationEventHandler : IIntegrationEventHandler<DuelStartIntegrationEvent>
    {
        #region Private variables

        private readonly IDuelUserRepository _duelUserRepository;

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly ILogger<DuelStartIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public DuelStartIntegrationEventHandler(IDuelUserRepository duelUserRepository,
                                                IEventBus eventBus,
                                                IDuelRepository duelRepository,
                                                IContestDateRepository contestDateRepository,
                                                ILogger<DuelStartIntegrationEventHandler> logger)
        {
            _duelUserRepository = duelUserRepository ?? throw new ArgumentNullException(nameof(duelUserRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this._duelRepository = duelRepository;
            _contestDateRepository = contestDateRepository;
        }

        #endregion Constructor

        #region Methos

        public async Task Handle(DuelStartIntegrationEvent @event)
        {
            short contestDateId = _contestDateRepository.ActiveContestDate().ContestDateId;

            bool isSuccess = await _duelRepository.Insert(new Infrastructure.Tables.Duel
            {
                BalanceType = @event.BalanceType,
                Bet = @event.Bet,
                SubCategoryId = @event.SubCategoryId,
                OpponentUserId = @event.OpponentUserId,
                FounderUserId = @event.FounderUserId,
                ContestDateId = contestDateId,
            });
            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Düello başlatılamadı.");

                SendErrorMessage(@event.FounderUserId, "düello hatası düello iptal");
                SendErrorMessage(@event.OpponentUserId, "düello hatası düello iptal");

                return;
            }
        }

        private void SendErrorMessage(string userId, string message)
        {
            var @event = new SendErrorMessageWithSignalrIntegrationEvent(userId, message);
            _eventBus.Publish(@event);
        }

        #endregion Methos
    }
}
