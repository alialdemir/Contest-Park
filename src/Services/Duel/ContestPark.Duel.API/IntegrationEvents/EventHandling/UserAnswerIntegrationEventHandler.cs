using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail;
using ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer;
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
    public class UserAnswerIntegrationEventHandler : IIntegrationEventHandler<UserAnswerIntegrationEvent>
    {
        #region Private Variables

        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IDuelDetailRepository _duelDetailRepository;
        private readonly IDuelRepository _duelRepository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<UserAnswerIntegrationEventHandler> _logger;
        private const byte MAX_ANSWER_COUNT = 7;

        #endregion Private Variables

        #region Constructor

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

        private DuelWinStatusModel _winStatus;

        /// <summary>
        /// Kazanma Kaybetme durumu
        /// </summary>
        private DuelWinStatusModel WinStatus
        {
            get

            {
                if (_winStatus == null)
                    _winStatus = _duelRepository.WinStatus(RealUserId);

                return _winStatus;
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
                "Soru cevaplandı. {DuelId} {QuestionId} {UserId} {Stylish} {BalanceType} {Time}",
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
                Answer();

                if (WinStatus.Check1 || (WinStatus.Check4 && Event.BalanceType == BalanceTypes.Money && !WinStatus.Check3))// Player yenildiği durumlar
                {
                    if ((IsFounderBot && (FounderTotalScore == 0 || OpponentTotalScore > FounderTotalScore))
                        || (IsOpponentBot && (OpponentTotalScore == 0 || FounderTotalScore > OpponentTotalScore)))// Sürekli soruyu doğru cevaplamasın diye puan farkı varsa doğru cevaplasın
                    {
                        PlayerLose();
                    }
                }
                else if (WinStatus.Check3 || (WinStatus.Check2 && Event.BalanceType == BalanceTypes.Gold))// Player yendiği durumlar
                {
                    if ((IsOpponentBot && OpponentTotalScore > FounderTotalScore)// Sürekli
                        || (IsFounderBot && FounderTotalScore > OpponentTotalScore))
                    {
                        PlayerWin();
                    }
                }

                await Delay();
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
        /// Eğer bot geç cevap vermiş ise bekletme koyduk
        /// </summary>
        private async Task Delay()
        {
            if (IsFounderBot)
            {
                int diff = Event.Time - CurrentRound.FounderTime;
                if (diff > 0 && CurrentRound.FounderAnswer != CurrentRound.CorrectAnswer)
                    await Task.Delay(diff * 1000);
            }
            else if (IsOpponentBot)
            {
                int diff = Event.Time - CurrentRound.OpponentTime;
                if (diff > 0 && CurrentRound.OpponentAnswer != CurrentRound.CorrectAnswer)
                    await Task.Delay(diff * 1000);
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
        private void Answer()
        {
            #region Her zaman random bir cevap veriyor

            if (IsFounderBot)
            {
                CurrentRound.FounderTime = (byte)RandomScore;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderAnswer = (Stylish)new Random().Next(1, 4);
                CurrentRound.FounderScore = CurrentRound.CorrectAnswer == CurrentRound.FounderAnswer ? _scoreCalculator.Calculator(Round, CurrentRound.FounderTime) : (byte)0;
            }
            else if (IsOpponentBot)
            {
                CurrentRound.OpponentTime = (byte)RandomScore;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentAnswer = (Stylish)new Random().Next(1, 4);
                CurrentRound.OpponentScore = CurrentRound.CorrectAnswer == CurrentRound.OpponentAnswer ? _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime) : (byte)0;
            }

            #endregion Her zaman random bir cevap veriyor

            if (IsFounderBot && (FounderTotalScore == 0 || (OpponentTotalScore + CurrentRound.OpponentScore) > (FounderTotalScore + CurrentRound.FounderScore)))
            {
                CurrentRound.FounderTime = (byte)RandomScore;

                if (CurrentRound.FounderTime > 10 || CurrentRound.FounderTime <= 0)
                    CurrentRound.FounderTime = 10;

                CurrentRound.FounderScore = _scoreCalculator.Calculator(Round, CurrentRound.FounderTime);
                CurrentRound.FounderAnswer = CurrentRound.CorrectAnswer;
            }
            else if (IsOpponentBot && (OpponentTotalScore == 0 || (FounderTotalScore + CurrentRound.FounderScore) > (OpponentTotalScore + CurrentRound.OpponentScore)))
            {
                CurrentRound.OpponentTime = (byte)RandomScore;

                if (CurrentRound.OpponentTime > 10 || CurrentRound.OpponentTime <= 0)
                    CurrentRound.OpponentTime = 10;

                CurrentRound.OpponentScore = _scoreCalculator.Calculator(Round, CurrentRound.OpponentTime);
                CurrentRound.OpponentAnswer = CurrentRound.CorrectAnswer;
            }
        }

        /// <summary>
        /// Bot'un mutlaka oyuncudan daha yüksek puanla doğru cevaplamasını sağlar
        /// </summary>
        private void PlayerLose()
        {
            if (IsFounderBot)
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
            else if (IsOpponentBot)
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
        /// Bot'un mutlaka yanlış cevaplamasını sağlar
        /// </summary>
        private void PlayerWin()
        {
            byte[] answers = new byte[4] { 1, 2, 3, 4 };

            if (IsFounderBot)
            {
                CurrentRound.FounderTime = (byte)(Event.Time - 3);
                CurrentRound.FounderScore = 0;
                CurrentRound.FounderAnswer = (Stylish)answers// Random yanlış cevap verdik
                                                        .Where(stylish => stylish != (byte)CurrentRound.CorrectAnswer)
                                                        .OrderBy(x => Guid.NewGuid())
                                                        .FirstOrDefault();
            }
            else if (IsOpponentBot)
            {
                CurrentRound.OpponentTime = (byte)(Event.Time - 3);
                CurrentRound.OpponentScore = 0;
                CurrentRound.OpponentAnswer = (Stylish)answers// Random yanlış cevap verdik
                                                        .Where(stylish => stylish != (byte)CurrentRound.CorrectAnswer)
                                                        .OrderBy(x => Guid.NewGuid())
                                                        .FirstOrDefault();
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
                                                                      CurrentRound.FounderUserId,
                                                                      CurrentRound.OpponentUserId,
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
