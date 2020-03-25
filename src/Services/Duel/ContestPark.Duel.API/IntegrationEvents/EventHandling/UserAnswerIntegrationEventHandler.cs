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
using System;
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
                @event.BalanceType,
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

            if (!(currentRound.FounderAnswer == Stylish.NotSeeQuestion || currentRound.FounderAnswer == Stylish.UnableToReply)
                && !(currentRound.OpponentAnswer == Stylish.NotSeeQuestion || currentRound.OpponentAnswer == Stylish.UnableToReply))
                return;

            int round = userAnswers.FindIndex(x => x.QuestionId == @event.QuestionId) + 1;// Question id'ye ait index bulukduğu roundu verir

            bool isFounder = @event.UserId == currentRound.FounderUserId;

            bool isCorrectAnswer = currentRound.CorrectAnswer == @event.Stylish;

            byte score = isCorrectAnswer ? _scoreCalculator.Calculator(round, @event.Time) : (byte)0;

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

            #region Kazanma ayarları

            #region Cevaplama

            bool isFounderBot = currentRound.FounderUserId.EndsWith("-bot");
            bool isOpponentBot = currentRound.OpponentUserId.EndsWith("-bot");

            if (isFounderBot || isOpponentBot)// Eğer bot ile oynanıyorsa ve bot cevaplamış ise
            {
                int rndScore = new Random().Next(5, 10);

                if (isFounderBot)
                {
                    currentRound.FounderTime = (byte)rndScore;

                    if (currentRound.FounderTime > 10 || currentRound.FounderTime <= 0)
                        currentRound.FounderTime = 10;

                    currentRound.FounderAnswer = (Stylish)new Random().Next(1, 4);
                    currentRound.FounderScore = currentRound.CorrectAnswer == currentRound.FounderAnswer ? _scoreCalculator.Calculator(round, currentRound.FounderTime) : (byte)0;

                    int diff = @event.Time - currentRound.FounderTime;
                    if (diff > 0 && currentRound.FounderAnswer != currentRound.CorrectAnswer)
                        await Task.Delay(diff * 1000);
                }
                else if (isOpponentBot)
                {
                    currentRound.OpponentTime = (byte)rndScore;

                    if (currentRound.OpponentTime > 10 || currentRound.OpponentTime <= 0)
                        currentRound.OpponentTime = 10;

                    currentRound.OpponentAnswer = (Stylish)new Random().Next(1, 4);
                    currentRound.OpponentScore = currentRound.CorrectAnswer == currentRound.OpponentAnswer ? _scoreCalculator.Calculator(round, currentRound.OpponentTime) : (byte)0;

                    int diff = @event.Time - currentRound.OpponentTime;
                    if (diff > 0 && currentRound.OpponentAnswer != currentRound.CorrectAnswer)
                        await Task.Delay(diff * 1000);
                }

                byte founderTotalScore = (byte)userAnswers.Sum(x => x.FounderScore);
                byte opponentTotalScore = (byte)userAnswers.Sum(x => x.OpponentScore);

                string realUserId = isFounderBot ? currentRound.OpponentUserId : currentRound.FounderUserId;// Bot olmayan kullanıcının user id
                string botUserId = isFounderBot ? currentRound.FounderUserId : currentRound.OpponentUserId;// bot kullanıcın id'si

                if (botUserId == currentRound.FounderUserId && (founderTotalScore == 0 || opponentTotalScore > founderTotalScore))
                {
                    currentRound.FounderTime = (byte)rndScore;

                    if (currentRound.FounderTime > 10 || currentRound.FounderTime <= 0)
                        currentRound.FounderTime = 10;

                    currentRound.FounderScore = _scoreCalculator.Calculator(round, currentRound.FounderTime);
                    currentRound.FounderAnswer = currentRound.CorrectAnswer;

                    _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                }
                else if (botUserId == currentRound.OpponentUserId && (opponentTotalScore == 0 || founderTotalScore > opponentTotalScore))
                {
                    currentRound.OpponentTime = (byte)rndScore;

                    if (currentRound.OpponentTime > 10 || currentRound.OpponentTime <= 0)
                        currentRound.OpponentTime = 10;

                    currentRound.OpponentScore = _scoreCalculator.Calculator(round, currentRound.OpponentTime);
                    currentRound.OpponentAnswer = currentRound.CorrectAnswer;

                    _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                }

                #endregion Cevaplama

                #region Para

                _logger.LogInformation("Düello bakiye tipi {BalanceType}", @event.BalanceType);

                if (@event.BalanceType == BalanceTypes.Money)// Eğer para ile oynanıyorsa ve bot cevaplamış ise
                {
                    BalanceModel balance = await _balanceService.GetBalance(realUserId, BalanceTypes.Money);// bot olmayan kullanıcının para miktarını aldık

                    _logger.LogInformation("Para ile düello oynanıyor. Oyuncunun şuanki para miktarı {balance} {realUserId}", balance.Amount, realUserId);

                    decimal maxMoney = 30.00m;// Convert.ToDecimal(new Random().Next(50, 70));

                    bool withdrawalStatus = balance.Amount >= maxMoney;// Oyunun para miktarı maxMoney'den fazla ise parayı her an çekebilir

                    if (withdrawalStatus && botUserId == currentRound.FounderUserId && opponentTotalScore > founderTotalScore)// Eğer bot kurucu ise rakip kazanıyorsa ve para çekmeye yakın ise
                    {
                        currentRound.FounderTime = currentRound.OpponentTime > 0
                            ? (byte)(currentRound.OpponentTime + rndScore)
                            : (byte)10;

                        if (currentRound.FounderTime > 10 || currentRound.FounderTime <= 0)
                            currentRound.FounderTime = 10;

                        currentRound.FounderScore = _scoreCalculator.Calculator(round, currentRound.FounderTime);
                        currentRound.FounderAnswer = currentRound.CorrectAnswer;

                        _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                    }
                    else if (withdrawalStatus && botUserId == currentRound.OpponentUserId && founderTotalScore > opponentTotalScore)// Eğer bot rakip ise kurucu kazanıyorsa ve para çekmeye yakın ise
                    {
                        currentRound.OpponentTime = currentRound.FounderTime > 0
                            ? (byte)(currentRound.FounderTime + rndScore)
                            : (byte)10;

                        if (currentRound.OpponentTime > 10 || currentRound.OpponentTime <= 0)
                            currentRound.OpponentTime = 10;

                        currentRound.OpponentScore = _scoreCalculator.Calculator(round, currentRound.OpponentTime);
                        currentRound.OpponentAnswer = currentRound.CorrectAnswer;

                        _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", currentRound.FounderScore, currentRound.OpponentScore);
                    }
                }

                #endregion Para
            }

            #endregion Kazanma ayarları

            userAnswers[round - 1] = currentRound;// Şuandaki round bilgileri aynı indexe set edildi

            if (currentRound.FounderAnswer == Stylish.NotSeeQuestion || currentRound.OpponentAnswer == Stylish.NotSeeQuestion)
            {
                _logger.LogError("Kullanıcı soruyu cevaplamamış gözüküyor", currentRound);

                return;
            }

            _userAnswerRepository.AddRangeAsync(userAnswers);// Redisdeki duello bilgileri tekrar update edildi

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
                                                                      isOpponentFinishedTheGame,
                                                                      isDuelCancel: false);

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
