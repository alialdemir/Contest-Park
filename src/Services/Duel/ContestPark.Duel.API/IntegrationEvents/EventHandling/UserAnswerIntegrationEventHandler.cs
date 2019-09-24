using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.ScoreCalculator;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class UserAnswerIntegrationEventHandler : IIntegrationEventHandler<UserAnswerIntegrationEvent>
    {
        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IDuelDetailRepository _duelDetailRepository;
        private readonly IDuelRepository _duelRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<UserAnswerIntegrationEventHandler> _logger;
        private const byte MAX_ANSWER_COUNT = 7;

        public UserAnswerIntegrationEventHandler(IUserAnswerRepository userAnswerRepository,
                                                 IScoreCalculator scoreCalculator,
                                                 IDuelDetailRepository duelDetailRepository,
                                                 IDuelRepository duelRepository,
                                                 IEventBus eventBus,
                                                 ILogger<UserAnswerIntegrationEventHandler> logger)
        {
            _userAnswerRepository = userAnswerRepository;
            _scoreCalculator = scoreCalculator;
            _duelDetailRepository = duelDetailRepository;
            _duelRepository = duelRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcının verdiği cevap doğru mu kontrol eder puanlayıp redise yazar
        /// iki kullanıcıda soruyu cevapladıysa ikisinede cevapları gönderir
        ///
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(UserAnswerIntegrationEvent @event)
        {
            _logger.LogInformation($@"Soru cevaplandı. Duel id: {@event.DuelId}
                                                       Question id: {@event.QuestionId}
                                                       User id: {@event.UserId}
                                                       Stylish: {@event.Stylish}
                                                       Time: {@event.Time}");

            List<UserAnswerModel> userAnswers = _userAnswerRepository.GetAnswers(@event.DuelId);
            if (userAnswers == null || userAnswers.Count == 0)
            {
                _logger.LogWarning("Düello cevapları rediste bulunamadı...");
                return Task.CompletedTask;
            }

            UserAnswerModel currentRound = userAnswers.FirstOrDefault(x => x.QuestionId == @event.QuestionId);
            if (currentRound == null)
            {
                // TODO: Düelloda hata oluştu iptal et paraları geri ver
            }

            // TODO: düelloda aynı soru kesinlikle iki kere gelmemeli
            int round = userAnswers.FindIndex(x => x.QuestionId == @event.QuestionId) + 1;// Question id'ye ait index bulukduğu roundu verir

            bool isFounder = @event.UserId == currentRound.FounderUserId;

            byte score = currentRound.CorrectAnswer == @event.Stylish ? _scoreCalculator.Calculator(round, @event.Time) : (byte)0;

            if (isFounder)//  Kurucu ise kurucuya puan verildi
            {
                currentRound.FounderAnswer = @event.Stylish;
                currentRound.FounderTime = @event.Time;
                currentRound.FounderScore = score;
            }
            else// Rakip ise rakibe puan verildi
            {
                currentRound.OpponentAnswer = @event.Stylish;
                currentRound.OpponentTime = @event.Time;
                currentRound.OpponentScore = score;
            }

            userAnswers[round - 1] = currentRound;// Şuandaki round bilgileri aynı indexe set edildi

            _userAnswerRepository.AddRangeAsync(userAnswers);// Redisdeki duello bilgileri tekrar update edildi

            if (currentRound.FounderAnswer == Enums.Stylish.NotSeeQuestion || currentRound.OpponentAnswer == Enums.Stylish.NotSeeQuestion)
                return Task.CompletedTask;

            Task.Factory.StartNew(async () =>// İki oyuncuda soruyu cevaplamışsa ikisinede verdikleri cevapları ve puanları gönderiyoruz
            {
                bool isGameEnd = round == MAX_ANSWER_COUNT;

                byte nextRound = (byte)(round + 1);// Sonraki raunda geçiildi

                PublishNextQuestionEvent(currentRound, @event.DuelId, isGameEnd, nextRound);

                if (isGameEnd)
                {
                    await SaveDuelDetailTable(@event.DuelId, userAnswers);
                }
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Redisdeki duello bilgisini tekrar ekleyerek güncelelr(aynı key'de ekleme yapınca güncellemiş oluyoruz)
        /// </summary>
        /// <param name="duelId">Duel id</param>
        /// <param name="userAnswers">Duello soru ve cevap bilgileri</param>
        private async Task SaveDuelDetailTable(int duelId, List<UserAnswerModel> userAnswers)
        {
            bool isSuccess = await _duelDetailRepository.AddRangeAsync(userAnswers.Select(x => new Infrastructure.Tables.DuelDetail
            {
                DuelId = x.DuelId,
                QuestionId = x.QuestionId,
                CorrectAnswer = x.CorrectAnswer,
                FounderAnswer = x.FounderAnswer,
                FounderScore = x.FounderScore,
                FounderTime = x.FounderTime,
                OpponentAnswer = x.OpponentAnswer,
                OpponentScore = x.OpponentScore,
                OpponentTime = x.OpponentTime,
            }).ToList());

            if (isSuccess)
            {
                UserAnswerModel firstItem = userAnswers.FirstOrDefault();// Kullanıcı idlerini alabilmek için ilk itemi aldım

                byte founderTotalScore = (byte)userAnswers.Sum(x => x.FounderScore);
                byte opponentTotalScore = (byte)userAnswers.Sum(x => x.OpponentScore);

                bool isFounderFinishedTheGame = userAnswers.Count(x => x.FounderAnswer != Enums.Stylish.NotSeeQuestion) == MAX_ANSWER_COUNT;
                bool isOpponentFinishedTheGame = userAnswers.Count(x => x.OpponentAnswer != Enums.Stylish.NotSeeQuestion) == MAX_ANSWER_COUNT;

                DuelBalanceInfoModel duelBalanceInfo = _duelRepository.GetDuelBalanceInfoByDuelId(duelId);

                _logger.LogInformation($@"Duello sona erdi. Duel id: {duelId}
                                                            BalanceType: {duelBalanceInfo.BalanceType}
                                                            Bet: {duelBalanceInfo.Bet}
                                                            Bet Commission: {duelBalanceInfo.BetCommission}
                                                            Sub Category Id: { duelBalanceInfo.SubCategoryId}
                                                            Founder User Id: {firstItem.FounderUserId}
                                                            Opponent User Id: {firstItem.OpponentUserId}
                                                            Founder Total Score: {founderTotalScore}
                                                            Opponent Total Score: {opponentTotalScore}");

                var @duelFinishEvent = new DuelFinishIntegrationEvent(duelId,
                                                                      duelBalanceInfo.BalanceType,
                                                                      duelBalanceInfo.Bet,
                                                                      duelBalanceInfo.BetCommission,
                                                                      duelBalanceInfo.SubCategoryId,
                                                                      firstItem.FounderUserId,
                                                                      firstItem.OpponentUserId,
                                                                      founderTotalScore,
                                                                      opponentTotalScore,
                                                                      isFounderFinishedTheGame,
                                                                      isOpponentFinishedTheGame);

                _eventBus.Publish(@duelFinishEvent);

                _userAnswerRepository.Remove(duelId);// redisdeki duello bilgileri silindi.
            }
            else
            {
                string duelloInfoJson = JsonConvert.SerializeObject(userAnswers);

                _logger.LogCritical($"CRITICAL: Duello bilgileri kayıt edilirken hata oluştu. {duelloInfoJson}", userAnswers);
            }
        }

        /// <summary>
        /// Kullanıcıların sorala verdikleri cevapları birbirine göndermek için event publish eder
        /// </summary>
        /// <param name="currentRound"></param>
        /// <param name="duelId"></param>
        /// <param name="isGameEnd"></param>
        /// <param name="nextRound"></param>
        private void PublishNextQuestionEvent(UserAnswerModel currentRound, int duelId, bool isGameEnd, byte nextRound)
        {
            var @nextQuestionEvent = new NextQuestionIntegrationEvent(duelId,
                                                                      currentRound.FounderAnswer,
                                                                      currentRound.OpponentAnswer,
                                                                      currentRound.CorrectAnswer,
                                                                      currentRound.FounderScore,
                                                                      currentRound.OpponentScore,
                                                                      nextRound,
                                                                      isGameEnd);

            _eventBus.Publish(@nextQuestionEvent);
        }
    }
}
