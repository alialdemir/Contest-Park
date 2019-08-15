using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.ScoreCalculator;
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
        private readonly IScoreCalculator _scoreCalculator;
        private readonly ILogger<DuelEscapeIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public DuelEscapeIntegrationEventHandler(IEventBus eventBus,
                                                 IDuelRepository duelRepository,
                                                 IScoreCalculator scoreCalculator,
                                                 ILogger<DuelEscapeIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _duelRepository = duelRepository;
            _scoreCalculator = scoreCalculator;
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

            //////////// List<Infrastructure.Tables.DuelDetail> duelDetails = ConvertToDuelDetail(@event.Questions, @event.DuelId);

            ////////////bool isSuccess = await _duelDetailRepository.AddRangeAsync(duelDetails);
            ////////////if (!isSuccess)
            ////////////{
            ////////////    _logger.LogError($"Düello detayı ekleme işlemi başarısız oldu. Duel Id: {@event.DuelId}");

            ////////////    SendErrorMessage(@event.EscaperUserId, "Düellodan çıkma işleminiz başarısız oldu.");

            ////////////    return;
            ////////////}

            duel.DuelType = isEscaperUserFounder ? DuelTypes.WinnerOpponent : DuelTypes.WinnerFounder;

            // Oyuncuların düellodan kaçma durumunda yenilmiş olma durumunu buradan ayarladık
            duel.FounderTotalScore = isEscaperUserFounder ? (byte)0 : (byte)@event.Questions.Sum(x => x.FounderScore);

            duel.OpponentTotalScore = !isEscaperUserFounder ? (byte)0 : (byte)@event.Questions.Sum(x => x.OpponentScore);

            #region Kazanma bonusları ver

            if (@event.EscaperUserId != duel.FounderUserId && duel.FounderTotalScore > duel.OpponentTotalScore)
            {
                duel.FounderVictoryScore += 70;// kurucu kazandıysa kurucuya +70 puan
            }
            else if (@event.EscaperUserId != duel.OpponentUserId && duel.OpponentTotalScore > duel.FounderTotalScore)
            {
                duel.OpponentVictoryScore += 70; // Rakip kazandıysa rakibe +70 puan
            }
            //else if (duel.FounderTotalScore == duel.OpponentTotalScore)
            //{
            //    duel.OpponentVictoryScore += 35;
            //    duel.OpponentVictoryScore += 35;
            //}

            #endregion Kazanma bonusları ver

            //#region Düello bitirme bonusları ver

            //if (duelDetails.Count == MAX_QUESTION_COUNT)
            //{
            //    duel.FounderFinshScore += 40;
            //    duel.OpponentFinshScore += 40;
            //}

            //#endregion Düello bitirme bonusları ver

            bool isSuccess = await _duelRepository.UpdateAsync(duel);
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
                                                                  duel.BetCommission,
                                                                  duel.SubCategoryId,
                                                                  duel.FounderUserId,
                                                                  duel.OpponentUserId,
                                                                  duel.FounderTotalScore ?? 0,
                                                                  duel.OpponentTotalScore ?? 0);

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
            for (int i = 0; i < duelQuestions.Count - 1; i++)
            {
                DuelFinishQuestionModel duelQuestion = duelQuestions[i];

                byte round = (byte)(i + 1);

                duelDetails.Add(new Infrastructure.Tables.DuelDetail
                {
                    DuelId = duelId,
                    QuestionId = duelQuestion.QuestionId,
                    CorrectAnswer = duelQuestion.CorrectAnswer,

                    FounderAnswer = duelQuestion.FounderAnswer,
                    FounderTime = duelQuestion.FounderTime,
                    FounderScore = duelQuestion.CorrectAnswer == duelQuestion.FounderAnswer ? _scoreCalculator.Calculator(round, duelQuestion.FounderTime) : (byte)0,// doğru cevap vermiş ise puan alıyor

                    OpponentScore = duelQuestion.CorrectAnswer == duelQuestion.OpponentAnswer ? _scoreCalculator.Calculator(round, duelQuestion.OpponentTime) : (byte)0,// doğru cevap vermiş ise puan alıyor
                    OpponentAnswer = duelQuestion.OpponentAnswer,
                    OpponentTime = duelQuestion.OpponentTime,
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
