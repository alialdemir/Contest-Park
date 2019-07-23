using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.Question;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class DuelStartIntegrationEventHandler : IIntegrationEventHandler<DuelStartIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly ILogger<DuelStartIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public DuelStartIntegrationEventHandler(IEventBus eventBus,
                                                IDuelRepository duelRepository,
                                                IQuestionRepository questionRepository,
                                                IContestDateRepository contestDateRepository,
                                                ILogger<DuelStartIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this._duelRepository = duelRepository;
            _questionRepository = questionRepository;
            _contestDateRepository = contestDateRepository;
        }

        #endregion Constructor

        #region Methos

        /// <summary>
        /// Kullanıcılar arasında düello başlat 7 soru çek kullanıcılara gönder
        /// </summary>
        /// <param name="event">Düello bilgileri</param>
        public async Task Handle(DuelStartIntegrationEvent @event)
        {
            ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();
            if (contestDate == null)
            {
                SendErrorMessage(@event.FounderUserId, "düello hatası düello iptal");
                SendErrorMessage(@event.OpponentUserId, "düello hatası düello iptal");

                return;
            }

            int? duelId = await _duelRepository.Insert(new Infrastructure.Tables.Duel
            {
                BalanceType = @event.BalanceType,
                Bet = @event.Bet,
                SubCategoryId = @event.SubCategoryId,
                OpponentUserId = @event.OpponentUserId,
                FounderUserId = @event.FounderUserId,
                ContestDateId = contestDate.ContestDateId
            });
            if (!duelId.HasValue)
            {
                _logger.LogCritical($@"CRITICAL: Düello başlatılamadı.
                                                 founder user id: {@event.FounderUserId}
                                                 founder user id: {@event.OpponentUserId}
                                                 balance type: {@event.BalanceType}
                                                 subCategory id: {@event.SubCategoryId}
                                                 bet: {@event.Bet}");

                SendErrorMessage(@event.FounderUserId, "düello hatası düello iptal");
                SendErrorMessage(@event.OpponentUserId, "düello hatası düello iptal");

                return;
            }

            var questions = await _questionRepository.DuelQuestions(@event.SubCategoryId,
                                                                    @event.FounderUserId,
                                                                    @event.OpponentUserId,
                                                                    @event.FounderLanguage,
                                                                    @event.OpponentLanguage);

            var @duelEvent = new DuelCreatedIntegrationEvent(duelId,
                                                             questions);
        }

        private void SendErrorMessage(string userId, string message)
        {
            var @event = new SendErrorMessageWithSignalrIntegrationEvent(userId, message);
            _eventBus.Publish(@event);
        }

        #endregion Methos
    }
}
