using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class DuelEscapeIntegrationEventHandler : IIntegrationEventHandler<DuelEscapeIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IDuelDetailRepository _duelDetailRepository;
        private readonly ILogger<DuelEscapeIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public DuelEscapeIntegrationEventHandler(IEventBus eventBus,
                                                 IDuelRepository duelRepository,
                                                 IDuelDetailRepository duelDetailRepository,
                                                 ILogger<DuelEscapeIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _duelRepository = duelRepository;
            _duelDetailRepository = duelDetailRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düellodan çıkart
        /// </summary>
        /// <param name="event">Düello bilgileri</param>
        public async Task Handle(DuelEscapeIntegrationEvent @event)
        {
            Infrastructure.Tables.Duel duel = _duelRepository.GetDuelByDuelId(@event.DuelId);
            if (duel == null)
            {
                _logger.LogCritical($"Düello çıkma işlemi sırasında düello id'ye ait düello bulunamadı. Duel Id: {@event.DuelId} Escaper UserId: {@event.EscaperUserId}");

                SendErrorMessage(@event.EscaperUserId, "Düellodan çıkma işleminiz gerçekleşemedi.");

                return;
            }

            bool isEscaperUserFounder = @event.EscaperUserId == duel.FounderUserId;
            if (!(isEscaperUserFounder || @event.EscaperUserId == duel.OpponentUserId))
            {
                SendErrorMessage(@event.EscaperUserId, "Bu düello size ait değil");
            }

            string winnerUserId = isEscaperUserFounder ? duel.OpponentUserId : duel.FounderUserId;

            List<Infrastructure.Tables.DuelDetail> duelDetails = ConvertToDuelDetail(@event.Questions, @event.DuelId);

            bool isSuccess = await _duelDetailRepository.AddRangeAsync(duelDetails);
            if (!isSuccess)
            {
                _logger.LogError($"Düello detayı ekleme işlemi başarısız oldu. Duel Id: {@event.DuelId}");

                SendErrorMessage(@event.EscaperUserId, "Düellodan çıkma işleminiz başarısız oldu.");

                return;
            }

            duel.DuelType = isEscaperUserFounder ? DuelTypes.WinnerOpponent : DuelTypes.WinnerFounder;

            // Oyuncuların düellodan kaçma durumunda yenilmiş olma durumunu buradan ayarladık
            duel.FounderTotalScore = isEscaperUserFounder ? (byte)0 : (byte)@event.Questions.Sum(x => x.FounderScore);

            duel.OpponentTotalScore = !isEscaperUserFounder ? (byte)0 : (byte)@event.Questions.Sum(x => x.OpponentScore);

            isSuccess = await _duelRepository.UpdateAsync(duel);
            if (!isSuccess)
            {
                _logger.LogError($"Düello detayı ekleme işlemi başarısız oldu. Duel Id: {@event.DuelId}");

                SendErrorMessage(@event.EscaperUserId, "Düellodan çıkma işleminiz başarısız oldu.");

                return;
            }

            // Düello bitti eventi yayınladık
            var @duelFinishEvent = new DuelFinishIntegrationEvent(@event.DuelId,
                                                                  duel.BalanceType,
                                                                  duel.Bet,
                                                                  duel.SubCategoryId,
                                                                  duel.FounderUserId,
                                                                  duel.OpponentUserId,
                                                                  duel.FounderTotalScore,
                                                                  duel.OpponentTotalScore);

            _eventBus.Publish(duelFinishEvent);
        }

        /// <summary>
        /// DuelFinishQuestionModel listesini alıp DuelDetail table modeline çevirir
        /// </summary>
        /// <param name="duelQuestions">Düelloda sorulan sorular ve verilen cevaplar</param>
        /// <param name="duelId">Düello id</param>
        /// <returns>DuelDetail list</returns>
        private List<Infrastructure.Tables.DuelDetail> ConvertToDuelDetail(List<DuelFinishQuestionModel> duelQuestions, int duelId)
        {
            List<Infrastructure.Tables.DuelDetail> duelDetails = new List<Infrastructure.Tables.DuelDetail>();

            foreach (var duelQueestion in duelQuestions)
            {
                duelDetails.Add(new Infrastructure.Tables.DuelDetail
                {
                    DuelId = duelId,
                    QuestionId = duelQueestion.QuestionId,
                    CorrectAnswer = duelQueestion.CorrectAnswer,

                    FounderAnswer = duelQueestion.FounderAnswer,
                    FounderScore = duelQueestion.FounderScore,
                    FounderTime = duelQueestion.FounderTime,

                    OpponentAnswer = duelQueestion.OpponentAnswer,
                    OpponentScore = duelQueestion.OpponentScore,
                    OpponentTime = duelQueestion.OpponentTime,
                });
            }

            return duelDetails;
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
