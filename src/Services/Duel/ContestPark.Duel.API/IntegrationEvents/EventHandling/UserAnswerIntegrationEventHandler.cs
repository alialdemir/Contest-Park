using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.Balance;
using ContestPark.Duel.API.Services.ScoreCalculator;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class UserAnswerIntegrationEventHandler : IIntegrationEventHandler<UserAnswerIntegrationEvent>
    {
        #region Private Variables

        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IDuelDetailRepository _duelDetailRepository;
        private readonly IDuelRepository _duelRepository;
        private readonly IEventBus _eventBus;
        private readonly IBalanceService _balanceService;
        private readonly ILogger<UserAnswerIntegrationEventHandler> _logger;
        private const byte MAX_ANSWER_COUNT = 7;

        #endregion Private Variables

        #region Constructor

        public UserAnswerIntegrationEventHandler(IUserAnswerRepository userAnswerRepository,
                                                 IScoreCalculator scoreCalculator,
                                                 IDuelDetailRepository duelDetailRepository,
                                                 IDuelRepository duelRepository,
                                                 IEventBus eventBus,
                                                 IBalanceService balanceService,
                                                 ILogger<UserAnswerIntegrationEventHandler> logger)
        {
            _userAnswerRepository = userAnswerRepository;
            _scoreCalculator = scoreCalculator;
            _duelDetailRepository = duelDetailRepository;
            _duelRepository = duelRepository;
            _eventBus = eventBus;
            _balanceService = balanceService;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcının verdiği cevap doğru mu kontrol eder puanlayıp redise yazar
        /// iki kullanıcıda soruyu cevapladıysa ikisinede cevapları gönderir
        ///
        /// </summary>
        public async Task Handle(UserAnswerIntegrationEvent @event)
        {
            _logger.LogInformation(
                "Soru cevaplandı. {DuelId} {QuestionId} {UserId} {Stylish} {Time}",
                @event.DuelId,
                @event.QuestionId,
                @event.UserId,
                @event.Stylish,
                @event.Time);

            List<UserAnswerModel> userAnswers = _userAnswerRepository.GetAnswers(@event.DuelId);
            if (userAnswers == null || userAnswers.Count == 0)
            {
                _logger.LogWarning("Düello cevapları rediste bulunamadı...");
                return;
            }

            UserAnswerModel currentRound = userAnswers.FirstOrDefault(x => x.QuestionId == @event.QuestionId);
            if (currentRound == null)
            {
                _logger.LogError("Round soru bilgisi boş geldi. {QuestionId}", @event.QuestionId);
                // TODO: Düelloda hata oluştu iptal et paraları geri ver
                return;
            }

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

            #region Bot kazanma ayarları

            if (@event.UserId.EndsWith("-bot"))// Eğer bot ile oynanıyorsa ve bot cevaplamış ise
            {
                _logger.LogInformation("Altın ile düello oynanıyor...");

                byte founderTotalScore = (byte)userAnswers.Sum(x => x.FounderScore);
                byte opponentTotalScore = (byte)userAnswers.Sum(x => x.OpponentScore);

                string realUserId = currentRound.FounderUserId.EndsWith("-bot") ? currentRound.OpponentUserId : currentRound.FounderUserId;// Bot olmayan kullanıcının user id
                string botUserId = currentRound.FounderUserId.EndsWith("-bot") ? currentRound.FounderUserId : currentRound.OpponentUserId;// bot kullanıcın id'si

                BalanceModel balance = await _balanceService.GetBalance(realUserId, BalanceTypes.Money);// bot olmayan kullanıcının Altın miktarını aldık
                bool withdrawalStatus = balance.Amount >= 80.00m;// Oyunun Altın miktarı 80'den fazla ise Altını her an çekebilir

                _logger.LogInformation("Oyuncunun şuanki Altın miktarı {balance} {realUserId}", balance.Amount, realUserId);

                if (withdrawalStatus && botUserId == currentRound.FounderUserId && opponentTotalScore > founderTotalScore)
                {
                    currentRound.FounderScore = @event.Time;
                    currentRound.FounderAnswer = currentRound.CorrectAnswer;

                    _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                }
                else if (withdrawalStatus && botUserId == currentRound.OpponentUserId && founderTotalScore > opponentTotalScore)
                {
                    currentRound.OpponentScore = @event.Time;
                    currentRound.OpponentAnswer = currentRound.CorrectAnswer;

                    _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                }
            }

            if (@event.BalanceType == BalanceTypes.Money && (currentRound.FounderUserId.EndsWith("-bot") || currentRound.OpponentUserId.EndsWith("-bot")))// Eğer para ile oynanıyorsa ve bot cevaplamış ise
            {
                _logger.LogInformation("Para ile düello oynanıyor...");

                byte founderTotalScore = (byte)userAnswers.Sum(x => x.FounderScore);
                byte opponentTotalScore = (byte)userAnswers.Sum(x => x.OpponentScore);

                string realUserId = currentRound.FounderUserId.EndsWith("-bot") ? currentRound.OpponentUserId : currentRound.FounderUserId;// Bot olmayan kullanıcının user id
                string botUserId = currentRound.FounderUserId.EndsWith("-bot") ? currentRound.FounderUserId : currentRound.OpponentUserId;// bot kullanıcın id'si

                BalanceModel balance = await _balanceService.GetBalance(realUserId, BalanceTypes.Money);// bot olmayan kullanıcının para miktarını aldık
                bool withdrawalStatus = balance.Amount >= 80.00m;// Oyunun para miktarı 80'den fazla ise parayı her an çekebilir

                _logger.LogInformation("Oyuncunun şuanki para miktarı {balance} {realUserId}", balance.Amount, realUserId);

                if (withdrawalStatus && botUserId == currentRound.FounderUserId && opponentTotalScore > founderTotalScore)// Eğer bot kurucu ise rakip kazanıyorsa ve para çekmeye yakın ise
                {
                    byte scoreDiff = (byte)(opponentTotalScore - founderTotalScore);
                    currentRound.FounderScore = (byte)(scoreDiff + 5);// Rakip ile kurucu arasındaki puan farkının +5
                    currentRound.FounderAnswer = currentRound.CorrectAnswer;

                    _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                }
                else if (withdrawalStatus && botUserId == currentRound.OpponentUserId && founderTotalScore > opponentTotalScore)// Eğer bot rakip ise kurucu kazanıyorsa ve para çekmeye yakın ise
                {
                    byte scoreDiff = (byte)(founderTotalScore - opponentTotalScore);
                    currentRound.OpponentScore = (byte)(scoreDiff + 5);// Kurucu ile rakip arasındaki puan farkının +5
                    currentRound.OpponentAnswer = currentRound.CorrectAnswer;

                    _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                }
            }

            #endregion Bot kazanma ayarları

            userAnswers[round - 1] = currentRound;// Şuandaki round bilgileri aynı indexe set edildi

            _userAnswerRepository.AddRangeAsync(userAnswers);// Redisdeki duello bilgileri tekrar update edildi

            if (currentRound.FounderAnswer == Stylish.NotSeeQuestion || currentRound.OpponentAnswer == Stylish.NotSeeQuestion)
                return;

            bool isGameEnd = round == MAX_ANSWER_COUNT;

            byte nextRound = (byte)(round + 1);// Sonraki raunda geçiildi

            PublishNextQuestionEvent(currentRound, @event.DuelId, isGameEnd, nextRound);

            if (isGameEnd)
            {
                await SaveDuelDetailTable(@event.DuelId, userAnswers);
            }
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

                _logger.LogInformation(
                    "Duello sona erdi. {duelId} {@duelBalanceInfo} {FounderUserId} {OpponentUserId} {founderTotalScore} {opponentTotalScore}",
                    duelId,
                    duelBalanceInfo,
                    firstItem.FounderUserId,
                    firstItem.OpponentUserId,
                    founderTotalScore,
                    opponentTotalScore);

                // İki oyuncuda soruyu cevaplamışsa ikisinede verdikleri cevapları ve puanları gönderiyoruz
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
                _logger.LogCritical("CRITICAL: Duello bilgileri kayıt edilirken hata oluştu. {@userAnswers}", userAnswers);
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

        #endregion Methods
    }
}
