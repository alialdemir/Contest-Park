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

        #region Properties

        private int DuelId { get; set; }
        private int QuestionId { get; set; }

        private UserAnswerModel _currentRound;

        private UserAnswerModel CurrentRound
        {
            get
            {
                if (_currentRound == null)
                    _currentRound = UserAnswers.FirstOrDefault(x => x.QuestionId == QuestionId);

                return _currentRound;
            }
        }

        private byte Round { get; set; }

        private List<UserAnswerModel> _userAnswers;

        private List<UserAnswerModel> UserAnswers
        {
            get
            {
                if (_userAnswers == null)
                    _userAnswers = _userAnswerRepository.GetAnswers(DuelId);

                return _userAnswers;
            }
        }

        private byte FounderTotalScore
        {
            get
            {
                return (byte)UserAnswers.Sum(x => x.FounderScore);
            }
        }

        private byte OpponentTotalScore
        {
            get
            {
                return (byte)UserAnswers.Sum(x => x.OpponentScore);
            }
        }

        private bool IsFounderBot
        {
            get
            {
                return CurrentRound.FounderUserId.EndsWith("-bot");
            }
        }

        private bool IsOpponentBot
        {
            get
            {
                return CurrentRound.OpponentUserId.EndsWith("-bot");
            }
        }

        /// <summary>
        /// Bot olmayan kullanıcının user id
        /// </summary>
        private string RealUserId
        {
            get
            {
                return IsFounderBot ? CurrentRound.OpponentUserId : CurrentRound.FounderUserId;
            }
        }

        /// <summary>
        /// Bot kullanıcın id'si
        /// </summary>
        private string BotUserId
        {
            get
            {
                return IsFounderBot ? CurrentRound.FounderUserId : CurrentRound.OpponentUserId;
            }
        }

        private bool? _winStatus;

        private bool WinStatus
        {
            get
            {
                if (!_winStatus.HasValue)
                    _winStatus = _duelRepository.WinStatus(RealUserId);

                return _winStatus.Value;
            }
        }

        private int RndScore
        {
            get
            {
                return new Random().Next(5, 10);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Kullanıcının verdiği cevap doğru mu kontrol eder puanlayıp redise yazar
        /// iki kullanıcıda soruyu cevapladıysa ikisinede cevapları gönderir
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

            DuelId = @event.DuelId;
            QuestionId = @event.QuestionId;

            if (UserAnswers == null || UserAnswers.Count == 0)
            {
                _logger.LogWarning("Düello cevapları rediste bulunamadı...");
                return;
            }

            if (CurrentRound == null)
            {
                _logger.LogError("Round soru bilgisi boş geldi. {QuestionId}", @event.QuestionId);
                // TODO: Düelloda hata oluştu iptal et paraları geri ver
                return;
            }

            if (!(CurrentRound.FounderAnswer == Stylish.NotSeeQuestion || CurrentRound.FounderAnswer == Stylish.UnableToReply)
                && !(CurrentRound.OpponentAnswer == Stylish.NotSeeQuestion || CurrentRound.OpponentAnswer == Stylish.UnableToReply))
                return;

            int round = UserAnswers.FindIndex(x => x.QuestionId == @event.QuestionId) + 1;// Question id'ye ait index bulukduğu roundu verir

            bool isFounder = @event.UserId == CurrentRound.FounderUserId;

            bool isCorrectAnswer = CurrentRound.CorrectAnswer == @event.Stylish;

            byte score = isCorrectAnswer ? _scoreCalculator.Calculator(round, @event.Time) : (byte)0;

            if (isFounder)//  Kurucu ise kurucuya puan verildi
            {
                CurrentRound.FounderAnswer = @event.Stylish;
                CurrentRound.FounderTime = @event.Time;
                CurrentRound.FounderScore = score;
            }
            else// Rakip ise rakibe puan verildi
            {
                CurrentRound.OpponentAnswer = @event.Stylish;
                CurrentRound.OpponentTime = @event.Time;
                CurrentRound.OpponentScore = score;
            }

            #region Oyun ayarları

            if (CurrentRound.FounderUserId.EndsWith("-bot") || CurrentRound.OpponentUserId.EndsWith("-bot"))// Eğer bot ile oynanıyorsa ve bot cevaplamış ise
            {
                await Answer(@event.Time);

                _logger.LogInformation("Düello bakiye tipi {BalanceType}", @event.BalanceType);

                if (@event.BalanceType == BalanceTypes.Gold)
                    await GoldGame(@event.Time);
                else if (@event.BalanceType == BalanceTypes.Money)
                    await MoneyGame(@event.Time);
            }

            #endregion Oyun ayarları

            UserAnswers[round - 1] = CurrentRound;// Şuandaki round bilgileri aynı indexe set edildi

            if (CurrentRound.FounderAnswer == Stylish.NotSeeQuestion || CurrentRound.OpponentAnswer == Stylish.NotSeeQuestion)
            {
                _logger.LogError("Kullanıcı soruyu cevaplamamış gözüküyor", CurrentRound);

                return;
            }

            _userAnswerRepository.AddRangeAsync(UserAnswers);// Redisdeki duello bilgileri tekrar update edildi

            bool isGameEnd = round == MAX_ANSWER_COUNT;

            byte nextRound = (byte)(round + 1);// Sonraki raunda geçiildi

            PublishNextQuestionEvent(CurrentRound, @event.DuelId, isGameEnd, nextRound);

            if (isGameEnd)
            {
                await SaveDuelDetailTable(@event.DuelId, UserAnswers);
            }
        }

        /// <summary>
        /// Oynama ayarlamaları
        /// </summary>
        /// <param name="time">Süre</param>
        private async Task Answer(byte time)
        {
            #region Cevaplama

            if (IsFounderBot)
            {
                CurrentRound.FounderTime = (byte)RndScore;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderAnswer = (Stylish)new Random().Next(1, 4);
                CurrentRound.FounderScore = CurrentRound.CorrectAnswer == CurrentRound.FounderAnswer ? _scoreCalculator.Calculator(Round, CurrentRound.FounderTime) : (byte)0;

                int diff = time - CurrentRound.FounderTime;
                if (diff > 0 && CurrentRound.FounderAnswer != CurrentRound.CorrectAnswer)
                    await Task.Delay(diff * 1000);
            }
            else if (IsOpponentBot)
            {
                CurrentRound.OpponentTime = (byte)RndScore;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentAnswer = (Stylish)new Random().Next(1, 4);
                CurrentRound.OpponentScore = CurrentRound.CorrectAnswer == CurrentRound.OpponentAnswer ? _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime) : (byte)0;

                int diff = time - CurrentRound.OpponentTime;
                if (diff > 0 && CurrentRound.OpponentAnswer != CurrentRound.CorrectAnswer)
                    await Task.Delay(diff * 1000);
            }

            byte founderTotalScore = (byte)UserAnswers.Sum(x => x.FounderScore);
            byte opponentTotalScore = (byte)UserAnswers.Sum(x => x.OpponentScore);

            string realUserId = IsFounderBot ? CurrentRound.OpponentUserId : CurrentRound.FounderUserId;// Bot olmayan kullanıcının user id
            string botUserId = IsFounderBot ? CurrentRound.FounderUserId : CurrentRound.OpponentUserId;// bot kullanıcın id'si

            if (botUserId == CurrentRound.FounderUserId && (founderTotalScore == 0 || opponentTotalScore > founderTotalScore))
            {
                CurrentRound.FounderTime = (byte)RndScore;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderScore = _scoreCalculator.Calculator(Round, CurrentRound.FounderTime);
                CurrentRound.FounderAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
            else if (botUserId == CurrentRound.OpponentUserId && (opponentTotalScore == 0 || founderTotalScore > opponentTotalScore))
            {
                CurrentRound.OpponentTime = (byte)RndScore;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentScore = _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime);
                CurrentRound.OpponentAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }

            #endregion Cevaplama
        }

        /// <summary>
        /// Altın ile oynama oyun ayarları
        /// </summary>
        /// <param name="time">Süre</param>
        private async Task GoldGame(byte time)
        {
            BalanceModel balance = await _balanceService.GetBalance(RealUserId, BalanceTypes.Gold);// bot olmayan kullanıcının para miktarını aldık

            _logger.LogInformation("Para ile düello oynanıyor. Oyuncunun şuanki para miktarı {balance} {realUserId}", balance.Amount, RealUserId);

            decimal maxGold = 1000.00m;

            bool withdrawalStatus = balance.Amount <= maxGold;// Oyunun para miktarı maxMoney'den fazla ise parayı her an çekebilir

            if (withdrawalStatus && RealUserId == CurrentRound.OpponentUserId && FounderTotalScore >= OpponentTotalScore)
            {
                FalseAnswer(true, time);
            }
            else if (withdrawalStatus && RealUserId == CurrentRound.FounderUserId && OpponentTotalScore >= FounderTotalScore)
            {
                FalseAnswer(false, time);
            }
            else if (WinStatus && BotUserId == CurrentRound.FounderUserId && OpponentTotalScore > FounderTotalScore)// Eğer bot kurucu ise rakip kazanıyorsa ve para çekmeye yakın ise
            {
                CurrentRound.FounderTime = CurrentRound.OpponentTime > 0
                    ? (byte)(CurrentRound.OpponentTime + RndScore)
                    : (byte)10;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderScore = _scoreCalculator.Calculator(Round, CurrentRound.FounderTime);
                CurrentRound.FounderAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
            else if (WinStatus && BotUserId == CurrentRound.OpponentUserId && FounderTotalScore > OpponentTotalScore)// Eğer bot rakip ise kurucu kazanıyorsa ve para çekmeye yakın ise
            {
                CurrentRound.OpponentTime = CurrentRound.FounderTime > 0
                    ? (byte)(CurrentRound.FounderTime + RndScore)
                    : (byte)10;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentScore = _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime);
                CurrentRound.OpponentAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
        }

        /// <summary>
        /// Para ile oynama oyun ayarları
        /// </summary>
        /// <param name="time">Süre</param>
        private async Task MoneyGame(byte time)
        {
            BalanceModel balance = await _balanceService.GetBalance(RealUserId, BalanceTypes.Money);// bot olmayan kullanıcının para miktarını aldık

            _logger.LogInformation("Para ile düello oynanıyor. Oyuncunun şuanki para miktarı {balance} {realUserId}", balance.Amount, RealUserId);

            decimal maxMoney = 70.00m;

            bool withdrawalStatus = balance.Amount >= maxMoney;// Oyunun para miktarı maxMoney'den fazla ise parayı her an çekebilir

            if ((WinStatus || withdrawalStatus) && BotUserId == CurrentRound.FounderUserId && OpponentTotalScore > FounderTotalScore)// Eğer bot kurucu ise rakip kazanıyorsa ve para çekmeye yakın ise
            {
                CurrentRound.FounderTime = CurrentRound.OpponentTime > 0
                    ? (byte)(CurrentRound.OpponentTime + RndScore)
                    : (byte)10;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderScore = _scoreCalculator.Calculator(Round, CurrentRound.FounderTime);
                CurrentRound.FounderAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
            else if ((WinStatus || withdrawalStatus) && BotUserId == CurrentRound.OpponentUserId && FounderTotalScore > OpponentTotalScore)// Eğer bot rakip ise kurucu kazanıyorsa ve para çekmeye yakın ise
            {
                CurrentRound.OpponentTime = CurrentRound.FounderTime > 0
                    ? (byte)(CurrentRound.FounderTime + RndScore)
                    : (byte)10;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentScore = _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime);
                CurrentRound.OpponentAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
            else if (balance.Amount <= 20.00m && RealUserId == CurrentRound.OpponentUserId && FounderTotalScore > OpponentTotalScore)
            {
                FalseAnswer(true, time);
            }
            else if (balance.Amount <= 20.00m && RealUserId == CurrentRound.FounderUserId && OpponentTotalScore > FounderTotalScore)
            {
                FalseAnswer(false, time);
            }
        }

        /// <summary>
        /// Soruyu mutlaka yanlış cevaplar
        /// </summary>
        /// <param name="isFounder">Kurucunun cevabı mı yanlış olacak yoksa rakibin mi/param>
        /// <param name="time">Süre</param>
        private void FalseAnswer(bool isFounder, byte time)
        {
            if (isFounder)
            {
                CurrentRound.FounderTime = (byte)(time - 3);
                CurrentRound.FounderScore = 0;

                switch (CurrentRound.CorrectAnswer)
                {
                    case Stylish.A:
                        CurrentRound.FounderAnswer = Stylish.B;
                        break;

                    case Stylish.B:
                        CurrentRound.FounderAnswer = Stylish.A;
                        break;

                    case Stylish.C:
                        CurrentRound.FounderAnswer = Stylish.D;
                        break;

                    case Stylish.D:
                        CurrentRound.FounderAnswer = Stylish.B;
                        break;
                }
            }
            else
            {
                CurrentRound.OpponentTime = (byte)(time - 3);
                CurrentRound.OpponentScore = 0;

                switch (CurrentRound.CorrectAnswer)
                {
                    case Stylish.A:
                        CurrentRound.OpponentAnswer = Stylish.B;
                        break;

                    case Stylish.B:
                        CurrentRound.OpponentAnswer = Stylish.A;
                        break;

                    case Stylish.C:
                        CurrentRound.OpponentAnswer = Stylish.D;
                        break;

                    case Stylish.D:
                        CurrentRound.OpponentAnswer = Stylish.B;
                        break;
                }
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
        /// <param name="CurrentRound"></param>
        /// <param name="duelId"></param>
        /// <param name="isGameEnd"></param>
        /// <param name="nextRound"></param>
        private void PublishNextQuestionEvent(UserAnswerModel CurrentRound, int duelId, bool isGameEnd, byte nextRound)
        {
            var @nextQuestionEvent = new NextQuestionIntegrationEvent(duelId,
                                                                      CurrentRound.FounderAnswer,
                                                                      CurrentRound.OpponentAnswer,
                                                                      CurrentRound.CorrectAnswer,
                                                                      CurrentRound.FounderScore,
                                                                      CurrentRound.OpponentScore,
                                                                      nextRound,
                                                                      isGameEnd);

            _eventBus.Publish(@nextQuestionEvent);
        }

        #endregion Methods
    }
}
