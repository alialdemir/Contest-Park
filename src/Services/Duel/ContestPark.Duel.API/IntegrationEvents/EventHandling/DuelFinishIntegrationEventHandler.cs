using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    /// <summary>
    /// Düello bittiğinde en son bu event tetiklenir
    /// </summary>
    public class DuelFinishIntegrationEventHandler : IIntegrationEventHandler<DuelFinishIntegrationEvent>
    {
        #region Private variables

        private readonly IEventBus _eventBus;
        private readonly IDuelRepository _duelRepository;
        private readonly IScoreRankingRepository _scoreRankingRepository;
        private readonly ILogger<DuelFinishIntegrationEventHandler> _logger;
        private readonly DuelSettings _duelSettings;
        private readonly byte finshBonus = 40;
        private readonly byte victoryBonus = 70;// 140

        #endregion Private variables

        #region Constructor

        public DuelFinishIntegrationEventHandler(IEventBus eventBus,
                                                 IDuelRepository duelRepository,
                                                 IScoreRankingRepository scoreRankingRepository,
                                                 IOptions<DuelSettings> settings,
                                                 ILogger<DuelFinishIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _duelRepository = duelRepository;
            _scoreRankingRepository = scoreRankingRepository;
            _duelSettings = settings.Value;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Herhangi bir şekilde(kazanma, kaybetme, beraberlik veya düellodan kaçma vb.) düello bittiğinde yapılacak işlemleri yapar
        /// </summary>
        /// <returns></returns>
        public async Task Handle(DuelFinishIntegrationEvent @event)
        {
            if (@event.FounderScore > 0)
            {
                bool isSuccess = await _scoreRankingRepository.UpdateScoreRank(@event.FounderUserId, @event.SubCategoryId, @event.BalanceType, @event.FounderScore);
                if (!isSuccess)
                {
                    _logger.LogError("Kullanıcıya skor eklenemedi. {FounderUserId} {SubCategoryId} {BalanceType} {FounderScore}",
                                     @event.FounderUserId,
                                     @event.SubCategoryId,
                                     @event.BalanceType,
                                     @event.FounderScore);
                }
            }

            if (@event.OpponentScore > 0)
            {
                bool isSuccess = await _scoreRankingRepository.UpdateScoreRank(@event.OpponentUserId, @event.SubCategoryId, @event.BalanceType, @event.OpponentScore);
                if (!isSuccess)
                {
                    _logger.LogError("Kullanıcıya skor eklenemedi. {FounderUserId} {SubCategoryId} {BalanceType} {FounderScore}",
                                     @event.OpponentUserId,
                                     @event.SubCategoryId,
                                     @event.BalanceType,
                                     @event.OpponentScore);
                }
            }

            #region Kazanma bonusu hesapla

            byte founderVictoryScore = 0;
            byte opponentVictoryScore = 0;

            DuelTypes duelType;
            if (@event.FounderScore > @event.OpponentScore) // Kurucu kazandıysa rakibe +70 puan
            {
                founderVictoryScore = victoryBonus;
                duelType = DuelTypes.WinnerFounder;
            }
            else if (@event.OpponentScore > @event.FounderScore) // Rakip kazandıysa rakibe +70 puan
            {
                opponentVictoryScore = victoryBonus;
                duelType = DuelTypes.WinnerOpponent;
            }
            else
            {
                founderVictoryScore = opponentVictoryScore = (byte)(victoryBonus / 2);
                duelType = DuelTypes.Draw;
            }

            #endregion Kazanma bonusu hesapla

            byte founderFinishedTheGameScore = @event.IsFounderFinishedTheGame ? finshBonus : (byte)0;
            byte opponentFinishedTheGameScore = @event.IsOpponentFinishedTheGame ? finshBonus : (byte)0;

            bool isSuccessDuelScoreUpdate = await _duelRepository.UpdateDuelScores(@event.DuelId,
                                                                                   duelType,
                                                                                   @event.FounderScore,
                                                                                   @event.OpponentScore,
                                                                                   founderFinishedTheGameScore,
                                                                                   opponentFinishedTheGameScore,
                                                                                   founderVictoryScore,
                                                                                   opponentVictoryScore);
            if (!isSuccessDuelScoreUpdate)
            {
                _logger.LogError("Duello bitirme skorları tablosya yazılamadı. {DuelId} {FounderScore} {OpponentScore} {finshBonus} {founderVictoryScore} {opponentVictoryScore}",
                                 @event.DuelId,
                                 @event.FounderScore,
                                 @event.OpponentScore,
                                 finshBonus,
                                 founderVictoryScore,
                                 opponentVictoryScore);
            }

            #region Kazanan kaybeden veya beraberlik belirleme(Düellolarda kazanılan bahisler buradan ayarlanıyor)

            // TODO: Kurucu ve rakip bakiyelerini ayrı ayrı düşmek yerine tek event de hallolucak şekilde yazılmalı rabbitmq da daha az event oluşur

            // TODO total skor kadar level xp'si eklensin

            if (@event.FounderScore == @event.OpponentScore)// Düello beraber bitmiş
            {
                ChangeBalance(@event.FounderUserId, @event.Bet, @event.BalanceType, BalanceHistoryTypes.Draw);

                ChangeBalance(@event.OpponentUserId, @event.Bet, @event.BalanceType, BalanceHistoryTypes.Draw);
            }
            else if (@event.FounderScore > @event.OpponentScore)// Kurucu düelloyu kazanmış
            {
                decimal founderBet = CaltulatorBetComission(@event.Bet, @event.BetCommission);

                ChangeBalance(@event.FounderUserId, founderBet, @event.BalanceType, BalanceHistoryTypes.Win);

                //  ChangeBalance(@event.OpponentUserId, 0, @event.BalanceType, BalanceHistoryTypes.Defeat);
            }
            else if (@event.OpponentScore > @event.FounderScore)// Rakip düelloyu kazanmış
            {
                decimal opponentBet = CaltulatorBetComission(@event.Bet, @event.BetCommission);

                // ChangeBalance(@event.FounderUserId, 0, @event.BalanceType, BalanceHistoryTypes.Defeat);

                ChangeBalance(@event.OpponentUserId, opponentBet, @event.BalanceType, BalanceHistoryTypes.Win);
            }

            #endregion Kazanan kaybeden veya beraberlik belirleme(Düellolarda kazanılan bahisler buradan ayarlanıyor)

            ChangedGameCount(@event.OpponentUserId);

            ChangedGameCount(@event.FounderUserId);

            AddPost(@event);
        }

        /// <summary>
        /// Bahisten komisyom düşer
        /// </summary>
        /// <param name="bet">Bahis miktarı</param>
        /// <returns>Komisyon düşülmüş bahis tutarı</returns>
        private decimal CaltulatorBetComission(decimal bet, byte betCommission)
        {
            bet = bet * 2; // Düellolar iki kişi oynandığı için  çarpı 2 yaptık yani bahisin iki katı kazandıdı

            decimal newBetCommission = bet - (bet * betCommission) / 100;// Bizim komisyon oranımız kadar kesinti yaptık

            return newBetCommission;
        }

        /// <summary>
        /// Post olarak ekleme eventi yollandı
        /// </summary>
        /// <param name="event">Düello bilgileri</param>
        private void AddPost(DuelFinishIntegrationEvent @event)
        {
            var @postEvent = new NewPostAddedIntegrationEvent(PostTypes.Contest,
                                                              @event.FounderUserId,
                                                              @event.Bet,
                                                              @event.BalanceType,
                                                              @event.OpponentUserId,
                                                              @event.OpponentScore,
                                                              @event.DuelId,
                                                              @event.FounderUserId,
                                                              @event.FounderScore,
                                                              @event.SubCategoryId);

            _eventBus.Publish(postEvent);
        }

        /// <summary>
        /// Oyuncunuun oyun oynama sayısını artırmak için event tetikler
        /// </summary>
        /// <param name="userId"></param>
        private void ChangedGameCount(string userId)
        {
            var @event = new ChangedGameCountIntegrationEvent(userId);

            _eventBus.Publish(@event);
        }

        /// <summary>
        /// Kullanıcının bakiyesini değiştirme eventi publish eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="bet">Bakiye</param>
        /// <param name="balanceType">Bakiye tipi</param>
        private void ChangeBalance(string userId, decimal bet, BalanceTypes balanceType, BalanceHistoryTypes balanceHistoryType)
        {
            if (bet <= 0)
                return;

            var @event = new ChangeBalanceIntegrationEvent(bet, userId, balanceType, balanceHistoryType);

            _eventBus.Publish(@event);
        }

        #endregion Methods
    }
}
