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

        /// <summary>
        /// Soruyu cevaplayanın bilgileri
        /// </summary>
        private UserAnswerIntegrationEvent Event { get; set; }

        /// <summary>
        /// Düello id
        /// </summary>

        private int DuelId
        {
            get
            {
                return Event.DuelId;
            }
        }

        /// <summary>
        /// Şuanki raundun soru id
        /// </summary>
        private int QuestionId
        {
            get
            {
                return Event.QuestionId;
            }
        }

        /// <summary>
        /// Şuanki Raund bilgisi
        /// </summary>
        private UserAnswerModel CurrentRound
        {
            get
            {
                return UserAnswers.FirstOrDefault(x => x.QuestionId == QuestionId);
            }
        }

        /// <summary>
        /// Question id'ye ait index'in bir fazlası bulukduğu roundu verir
        /// </summary>

        private int Round
        {
            get
            {
                return UserAnswers.FindIndex(x => x.QuestionId == QuestionId) + 1;
            }
        }

        /// <summary>
        /// Sonraki raunda sayısı
        /// </summary>
        private byte NextRound
        {
            get
            {
                return (byte)(Round + 1);
            }
        }

        private List<UserAnswerModel> _userAnswers;

        /// <summary>
        /// Oyuncuların cevapları
        /// </summary>
        private List<UserAnswerModel> UserAnswers
        {
            get
            {
                if (_userAnswers == null)
                    _userAnswers = _userAnswerRepository.GetAnswers(DuelId);

                return _userAnswers;
            }
        }

        /// <summary>
        /// Kurucu toplam skor
        /// </summary>
        private byte FounderTotalScore
        {
            get
            {
                return (byte)UserAnswers.Sum(x => x.FounderScore);
            }
        }

        /// <summary>
        /// Rakip toplam skor
        /// </summary>
        private byte OpponentTotalScore
        {
            get
            {
                return (byte)UserAnswers.Sum(x => x.OpponentScore);
            }
        }

        /// <summary>
        /// Kurucu kullanıcı id içinde -bot geçiyorsa true geçmiyorsa false
        /// </summary>
        private bool IsFounderBot
        {
            get
            {
                return CurrentRound.FounderUserId.EndsWith("-bot");
            }
        }

        /// <summary>
        /// Rakip kullanıcı id içinde -bot geçiyorsa true geçmiyorsa false
        /// </summary>
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

        private bool? _winStatus = null;

        /// <summary>
        /// Kazanma Kaybetme durumu
        /// </summary>
        private bool WinStatus
        {
            get

            {
                if (!_winStatus.HasValue)
                    _winStatus = _duelRepository.WinStatus(RealUserId);

                return _winStatus.Value;
            }
        }

        /// <summary>
        /// Random skor verir
        /// </summary>
        private int RandomScore
        {
            get
            {
                return new Random().Next(5, 10);
            }
        }

        /// <summary>
        /// Soruya cevap verenin cevabı doğru mu
        /// </summary>
        private bool IsCorrectAnswer
        {
            get
            {
                return CurrentRound.CorrectAnswer == Event.Stylish;
            }
        }

        /// <summary>
        /// Soruyu cevaplayan kurucu ise true değilse false
        /// </summary>
        private bool IsFounder
        {
            get
            {
                return Event.UserId == CurrentRound.FounderUserId;
            }
        }

        /// <summary>
        /// Düello bitti mi bilgisini verir true ise bitti false ise devam ediyor
        /// Maksimum MAX_ANSWER_COUNT kadar olabilir
        /// </summary>
        private bool IsGameEnd
        {
            get
            {
                return Round == MAX_ANSWER_COUNT;
            }
        }

        /// <summary>
        /// Soruya cevap verenin aldığı puan
        /// </summary>
        private byte CurrentAnswerScore
        {
            get
            {
                return IsCorrectAnswer
                    ? _scoreCalculator.Calculator(Round, Event.Time)
                    : (byte)0;
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

            Event = @event;

            if (UserAnswers == null || UserAnswers.Count == 0)
            {
                _logger.LogWarning("Düello cevapları rediste bulunamadı...");

                CancelDuel();

                return;
            }

            if (CurrentRound == null)
            {
                _logger.LogError("Round soru bilgisi boş geldi. {QuestionId}", @event.QuestionId);

                CancelDuel();

                return;
            }

            if (!(CurrentRound.FounderAnswer == Stylish.NotSeeQuestion || CurrentRound.FounderAnswer == Stylish.UnableToReply)
                && !(CurrentRound.OpponentAnswer == Stylish.NotSeeQuestion || CurrentRound.OpponentAnswer == Stylish.UnableToReply))
                return;

            if (IsFounder)//  Kurucu ise kurucuya puan verildi
            {
                CurrentRound.FounderAnswer = @event.Stylish;
                CurrentRound.FounderTime = @event.Time;
                CurrentRound.FounderScore = CurrentAnswerScore;
            }
            else// Rakip ise rakibe puan verildi
            {
                CurrentRound.OpponentAnswer = @event.Stylish;
                CurrentRound.OpponentTime = @event.Time;
                CurrentRound.OpponentScore = CurrentAnswerScore;
            }

            #region Oyun ayarları

            if (IsFounderBot || IsOpponentBot)// Eğer bot ile oynanıyorsa ve bot cevaplamış ise
            {
                await Answer();

                if (@event.BalanceType == BalanceTypes.Gold)
                    await GoldGame();
                else if (@event.BalanceType == BalanceTypes.Money)
                    await MoneyGame();
            }

            #endregion Oyun ayarları

            UserAnswers[Round - 1] = CurrentRound;// Şuandaki round bilgileri aynı indexe set edildi

            if (CurrentRound.FounderAnswer == Stylish.NotSeeQuestion
                || CurrentRound.OpponentAnswer == Stylish.NotSeeQuestion)
            {
                _logger.LogError("Kullanıcı soruyu cevaplamamış gözüküyor", CurrentRound);

                return;
            }

            _userAnswerRepository.AddRangeAsync(UserAnswers);// Redisdeki duello bilgileri tekrar update edildi

            PublishNextQuestionEvent();

            if (IsGameEnd)
            {
                await SaveDuelDetailTable();
            }
        }

        /// <summary>
        /// Düelloyu iptal eder
        /// </summary>
        private void CancelDuel()
        {
            var @event = new DuelEscapeIntegrationEvent(DuelId,
                                                        RealUserId,
                                                        isDuelCancel: true);

            _eventBus.Publish(@event);
        }

        /// <summary>
        /// Oynama ayarlamaları
        /// </summary>
        /// <param name="time">Süre</param>
        private async Task Answer()
        {
            if (IsFounderBot)
            {
                CurrentRound.FounderTime = (byte)RandomScore;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderAnswer = (Stylish)new Random().Next(1, 4);
                CurrentRound.FounderScore = CurrentRound.CorrectAnswer == CurrentRound.FounderAnswer ? _scoreCalculator.Calculator(Round, CurrentRound.FounderTime) : (byte)0;

                int diff = Event.Time - CurrentRound.FounderTime;
                if (diff > 0 && CurrentRound.FounderAnswer != CurrentRound.CorrectAnswer)
                    await Task.Delay(diff * 1000);
            }
            else if (IsOpponentBot)
            {
                CurrentRound.OpponentTime = (byte)RandomScore;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentAnswer = (Stylish)new Random().Next(1, 4);
                CurrentRound.OpponentScore = CurrentRound.CorrectAnswer == CurrentRound.OpponentAnswer ? _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime) : (byte)0;

                int diff = Event.Time - CurrentRound.OpponentTime;
                if (diff > 0 && CurrentRound.OpponentAnswer != CurrentRound.CorrectAnswer)
                    await Task.Delay(diff * 1000);
            }

            if (BotUserId == CurrentRound.FounderUserId && (FounderTotalScore == 0 || OpponentTotalScore > FounderTotalScore))
            {
                CurrentRound.FounderTime = (byte)RandomScore;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderScore = _scoreCalculator.Calculator(Round, CurrentRound.FounderTime);
                CurrentRound.FounderAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot kurucu ve rakip kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
            else if (BotUserId == CurrentRound.OpponentUserId && (OpponentTotalScore == 0 || FounderTotalScore > OpponentTotalScore))
            {
                CurrentRound.OpponentTime = (byte)RandomScore;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentScore = _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime);
                CurrentRound.OpponentAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
        }

        /// <summary>
        /// Altın ile oynama oyun ayarları
        /// </summary>
        /// <param name="time">Süre</param>
        private async Task GoldGame()
        {
            BalanceModel balance = await _balanceService.GetBalance(RealUserId, BalanceTypes.Gold);// bot olmayan kullanıcının para miktarını aldık

            _logger.LogInformation("Para ile düello oynanıyor. Oyuncunun şuanki para miktarı {balance} {realUserId}", balance.Amount, RealUserId);

            decimal maxGold = 1000.00m;

            bool withdrawalStatus = balance.Amount <= maxGold;// Oyunun para miktarı maxMoney'den fazla ise parayı her an çekebilir

            if (!WinStatus && withdrawalStatus && RealUserId == CurrentRound.OpponentUserId && FounderTotalScore >= OpponentTotalScore)
            {
                FalseAnswer(true);
            }
            else if (!WinStatus && withdrawalStatus && RealUserId == CurrentRound.FounderUserId && OpponentTotalScore >= FounderTotalScore)
            {
                FalseAnswer(false);
            }
            else if (WinStatus && BotUserId == CurrentRound.FounderUserId && OpponentTotalScore > FounderTotalScore)// Eğer bot kurucu ise rakip kazanıyorsa ve para çekmeye yakın ise
            {
                CurrentRound.FounderTime = CurrentRound.OpponentTime > 0
                    ? (byte)(CurrentRound.OpponentTime + RandomScore)
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
                    ? (byte)(CurrentRound.FounderTime + RandomScore)
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
        private async Task MoneyGame()
        {
            BalanceModel balance = await _balanceService.GetBalance(RealUserId, BalanceTypes.Money);// bot olmayan kullanıcının para miktarını aldık

            _logger.LogInformation("Para ile düello oynanıyor. Oyuncunun şuanki para miktarı {balance} {realUserId}", balance.Amount, RealUserId);

            decimal maxMoney = 70.00m;

            bool withdrawalStatus = balance.Amount >= maxMoney;// Oyunun para miktarı maxMoney'den fazla ise parayı her an çekebilir

            if ((WinStatus || withdrawalStatus) && BotUserId == CurrentRound.FounderUserId && OpponentTotalScore > FounderTotalScore)// Eğer bot kurucu ise rakip kazanıyorsa ve para çekmeye yakın ise
            {
                CurrentRound.FounderTime = CurrentRound.OpponentTime > 0
                    ? (byte)(CurrentRound.OpponentTime + RandomScore)
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
                    ? (byte)(CurrentRound.FounderTime + RandomScore)
                    : (byte)10;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentScore = _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime);
                CurrentRound.OpponentAnswer = CurrentRound.CorrectAnswer;

                _logger.LogInformation("Bot rakip ve kurucu kazanıyor. {FounderScore} {OpponentScore}", CurrentRound.FounderScore, CurrentRound.OpponentScore);
            }
        }

        /// <summary>
        /// Soruyu mutlaka yanlış cevaplar
        /// </summary>
        /// <param name="isFounder">Kurucunun cevabı mı yanlış olacak yoksa rakibin mi/param>
        /// <param name="time">Süre</param>
        private void FalseAnswer(bool isFounder)
        {
            if (isFounder)
            {
                CurrentRound.FounderTime = (byte)(Event.Time - 3);
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
                CurrentRound.OpponentTime = (byte)(Event.Time - 3);
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
        private async Task SaveDuelDetailTable()
        {
            _logger.LogInformation("Duello sona erdi. {duelId} {FounderUserId} {OpponentUserId} {FounderTotalScore} {OpponentTotalScore}",
                                   DuelId,
                                   CurrentRound.FounderUserId,
                                   CurrentRound.OpponentUserId,
                                   FounderTotalScore,
                                   OpponentTotalScore);

            bool isSuccess = await _duelDetailRepository.AddRangeAsync(UserAnswers.Select(x => new Infrastructure.Tables.DuelDetail
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

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Duello bilgileri kayıt edilirken hata oluştu. {DuelId}", DuelId);

                CancelDuel();

                return;
            }

            bool isFounderFinishedTheGame = UserAnswers.Count(x => x.FounderAnswer != Stylish.NotSeeQuestion) == MAX_ANSWER_COUNT;
            bool isOpponentFinishedTheGame = UserAnswers.Count(x => x.OpponentAnswer != Stylish.NotSeeQuestion) == MAX_ANSWER_COUNT;

            DuelBalanceInfoModel duelBalanceInfo = _duelRepository.GetDuelBalanceInfoByDuelId(DuelId);

            // İki oyuncuda soruyu cevaplamışsa ikisinede verdikleri cevapları ve puanları gönderiyoruz
            var @duelFinishEvent = new DuelFinishIntegrationEvent(DuelId,
                                                                  duelBalanceInfo.BalanceType,
                                                                  duelBalanceInfo.Bet,
                                                                  duelBalanceInfo.BetCommission,
                                                                  duelBalanceInfo.SubCategoryId,
                                                                  CurrentRound.FounderUserId,
                                                                  CurrentRound.OpponentUserId,
                                                                  FounderTotalScore,
                                                                  OpponentTotalScore,
                                                                  isFounderFinishedTheGame,
                                                                  isOpponentFinishedTheGame,
                                                                  isDuelCancel: false);

            _eventBus.Publish(@duelFinishEvent);

            _userAnswerRepository.Remove(DuelId);// redisdeki duello bilgileri silindi.
        }

        /// <summary>
        /// Kullanıcıların sorala verdikleri cevapları birbirine göndermek için event publish eder
        /// </summary>
        private void PublishNextQuestionEvent()
        {
            var @nextQuestionEvent = new NextQuestionIntegrationEvent(DuelId,
                                                                      CurrentRound.FounderAnswer,
                                                                      CurrentRound.OpponentAnswer,
                                                                      CurrentRound.CorrectAnswer,
                                                                      CurrentRound.FounderScore,
                                                                      CurrentRound.OpponentScore,
                                                                      NextRound,
                                                                      IsGameEnd);

            _eventBus.Publish(@nextQuestionEvent);
        }

        #endregion Methods
    }
}
