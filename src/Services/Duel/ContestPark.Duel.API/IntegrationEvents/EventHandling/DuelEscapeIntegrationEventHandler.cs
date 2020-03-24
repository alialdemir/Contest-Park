using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Services.ScoreCalculator;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class DuelEscapeIntegrationEventHandler : IIntegrationEventHandler<DuelEscapeIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IDuelRepository _duelRepository;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly ILogger<DuelEscapeIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public DuelEscapeIntegrationEventHandler(IEventBus eventBus,
                                                 IUserAnswerRepository userAnswerRepository,
                                                 IDuelRepository duelRepository,
                                                 IScoreCalculator scoreCalculator,
                                                 ILogger<DuelEscapeIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _userAnswerRepository = userAnswerRepository;
            _duelRepository = duelRepository;
            _scoreCalculator = scoreCalculator;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düellodan çıkart
        /// </summary>
        /// <param name="event">Düello bilgileri</param>
        public Task Handle(DuelEscapeIntegrationEvent @event)
        {
            Infrastructure.Tables.Duel duel = _duelRepository.GetDuelByDuelId(@event.DuelId);
            if (duel == null)
            {
                _logger.LogCritical("Düello çıkma işlemi sırasında düello id'ye ait düello bulunamadı. {DuelId} {EscaperUserId}",
                                    @event.DuelId,
                                    @event.EscaperUserId);

                SendErrorMessage(@event.EscaperUserId, "Düellodan çıkma işleminiz gerçekleşemedi.");

                return Task.CompletedTask;
            }

            bool isEscaperUserFounder = @event.EscaperUserId == duel.FounderUserId;
            if (!(isEscaperUserFounder || @event.EscaperUserId == duel.OpponentUserId))
            {
                SendErrorMessage(@event.EscaperUserId, "Bu düello size ait değil");

                return Task.CompletedTask;
            }

            var userAnswers = _userAnswerRepository.GetAnswers(@event.DuelId);
            if (!@event.IsDuelCancel && userAnswers != null && userAnswers.Count > 0)
            {
                /*
                 * Oyuncuların düellodan kaçma durumunda yenilmiş olma durumunu buradan ayarladık
                 * Eğer puanlar sıfır olursa berabere bitiyordu bu yüzden düelloden kaçmayan oyuncunun puanını +1 yaptık
                 */

                duel.FounderTotalScore = isEscaperUserFounder ? (byte)0 : (byte)(userAnswers.Sum(x => x.FounderScore) + 1);

                duel.OpponentTotalScore = !isEscaperUserFounder ? (byte)0 : (byte)(userAnswers.Sum(x => x.OpponentScore) + 1);
            }

            bool isFounderFinishedTheGame = false;
            bool isOpponentFinishedTheGame = false;

            #region Kazanma bonusu verilecek mi belirler

            if (!@event.IsDuelCancel && @event.EscaperUserId != duel.FounderUserId && duel.FounderTotalScore > duel.OpponentTotalScore)
            {
                isFounderFinishedTheGame = true;
            }
            else if (!@event.IsDuelCancel && @event.EscaperUserId != duel.OpponentUserId && duel.OpponentTotalScore > duel.FounderTotalScore)
            {
                isOpponentFinishedTheGame = true;
            }

            #endregion Kazanma bonusu verilecek mi belirler

            // Düello bitti eventi yayınladık
            var @duelFinishEvent = new DuelFinishIntegrationEvent(@event.DuelId,
                                                                  duel.BalanceType,
                                                                  duel.Bet,
                                                                  duel.BetCommission,
                                                                  duel.SubCategoryId,
                                                                  duel.FounderUserId,
                                                                  duel.OpponentUserId,
                                                                  duel.FounderTotalScore ?? 0,
                                                                  duel.OpponentTotalScore ?? 0,
                                                                  isFounderFinishedTheGame,
                                                                  isOpponentFinishedTheGame,
                                                                  @event.IsDuelCancel);

            _eventBus.Publish(duelFinishEvent);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Signalr ile clients hata mesajı gönderir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="message">Gönderilecek mesaj</param>
        private void SendErrorMessage(string userId, string message)
        {
            var @event = new SendErrorMessageWithSignalrIntegrationEvent(userId, message);

            _eventBus.Publish(@event);
        }

        #endregion Methods
    }
}
