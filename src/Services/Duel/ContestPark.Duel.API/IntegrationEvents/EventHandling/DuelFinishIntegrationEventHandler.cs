using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.Duel;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
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

        #endregion Private variables

        #region Constructor

        public DuelFinishIntegrationEventHandler(IEventBus eventBus,
                                                 IDuelRepository duelRepository,
                                                 IScoreRankingRepository scoreRankingRepository,
                                                 ILogger<DuelFinishIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _duelRepository = duelRepository;
            _scoreRankingRepository = scoreRankingRepository;
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
                    _logger.LogError($@"Kullanıcıya skor eklenemedi. user Id: {@event.FounderUserId}
                                                                     subCategory Id: {@event.SubCategoryId}
                                                                     balance Type: {@event.BalanceType}
                                                                     score: {@event.FounderScore}");
                }
            }

            if (@event.OpponentScore > 0)
            {
                bool isSuccess = await _scoreRankingRepository.UpdateScoreRank(@event.OpponentUserId, @event.SubCategoryId, @event.BalanceType, @event.OpponentScore);
                if (!isSuccess)
                {
                    _logger.LogError($@"Kullanıcıya skor eklenemedi. user Id: {@event.OpponentUserId}
                                                                     subCategory Id: {@event.SubCategoryId}
                                                                     balance Type: {@event.BalanceType}
                                                                     score: {@event.OpponentScore}");
                }
            }

            AddPost(@event);

            #region Kazanan kaybeden veya beraberlik belirleme(Düellolarda kazanılan bahisler buradan ayarlanıyor)

            // TODO: Kurucu ve rakip bakiyelerini ayrı ayrı düşmek yerine tek event de hallolucak şekilde yazılmalı rabbitmq da daha az event oluşur

            if (@event.FounderScore == @event.OpponentScore)// Düello beraber bitmiş
            {
                ChangeBalance(@event.FounderUserId, @event.Bet, @event.BalanceType, BalanceHistoryTypes.Draw);

                ChangeBalance(@event.OpponentUserId, @event.Bet, @event.BalanceType, BalanceHistoryTypes.Draw);
            }
            else if (@event.FounderScore > @event.OpponentScore)// Kurucu düelloyu kazanmış
            {
                decimal founderBet = @event.Bet * 2; // Düellolar iki kişi oynandığı için  çarpı 2 yaptık yani bahisin iki katı kazandıdı

                ChangeBalance(@event.FounderUserId, founderBet, @event.BalanceType, BalanceHistoryTypes.Win);

                ChangeBalance(@event.OpponentUserId, 0, @event.BalanceType, BalanceHistoryTypes.Defeat);
            }
            else if (@event.OpponentScore > @event.FounderScore)// Rakip düelloyu kazanmış
            {
                decimal opponentBet = @event.Bet * 2; // Düellolar iki kişi oynandığı için  çarpı 2 yaptık yani bahisin iki katı kazandıdı

                ChangeBalance(@event.FounderUserId, 0, @event.BalanceType, BalanceHistoryTypes.Defeat);

                ChangeBalance(@event.OpponentUserId, opponentBet, @event.BalanceType, BalanceHistoryTypes.Win);
            }

            #endregion Kazanan kaybeden veya beraberlik belirleme(Düellolarda kazanılan bahisler buradan ayarlanıyor)
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
                                                              @event.OpponentUserId,
                                                              @event.OpponentScore,
                                                              @event.DuelId,
                                                              @event.FounderUserId,
                                                              @event.FounderScore,
                                                              @event.SubCategoryId);

            _eventBus.Publish(postEvent);
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
