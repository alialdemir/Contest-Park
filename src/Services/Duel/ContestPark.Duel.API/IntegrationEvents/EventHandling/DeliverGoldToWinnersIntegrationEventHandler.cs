using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
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
    public class DeliverGoldToWinnersIntegrationEventHandler : IIntegrationEventHandler<DeliverGoldToWinnersIntegrationEvent>
    {
        #region Private variables

        private readonly IScoreRankingRepository _scoreRankingRepository;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly ILogger<DeliverGoldToWinnersIntegrationEventHandler> _logger;
        private readonly IEventBus _eventBus;

        #endregion Private variables

        #region Constructor

        public DeliverGoldToWinnersIntegrationEventHandler(IScoreRankingRepository scoreRankingRepository,
                                                           IContestDateRepository contestDateRepository,
                                                           ILogger<DeliverGoldToWinnersIntegrationEventHandler> logger,
                                                           IEventBus eventBus)
        {
            _scoreRankingRepository = scoreRankingRepository;
            _contestDateRepository = contestDateRepository;
            _logger = logger;
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Methods

        public async Task Handle(DeliverGoldToWinnersIntegrationEvent @event)
        {
            _logger.LogInformation("{contestDateId} numaralı yarışma kazananlarına altınlar dağıtılıyor.", @event.ContestDateId);

            ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();
            if (contestDate.FinishDate > DateTime.Now)
            {
                _logger.LogError("Süresi dolmamış yarışmanın ödülleri dağıtılmaya çalışıldı.");

                return;
            }

            BalanceTypes balanceGold = BalanceTypes.Gold;

            IEnumerable<WinnersModel> winners = _scoreRankingRepository.Winners(@event.ContestDateId,
                                                                                balanceGold);
            if (winners == null || !winners.Any())
            {
                _logger.LogWarning("{contestDateId} numaralı yarışmada kazanan bulunamadı.", @event.ContestDateId);

                return;
            }

            var @changeBalancesEvent = new MultiChangeBalanceIntegrationEvent();

            foreach (var item in winners)
            {
                @changeBalancesEvent.AddChangeBalance(new ChangeBalanceModel
                {
                    BalanceType = balanceGold,
                    BalanceHistoryType = BalanceHistoryTypes.CategoryWinner,
                    UserId = item.Premier,
                    Amount = 3000
                });

                @changeBalancesEvent.AddChangeBalance(new ChangeBalanceModel
                {
                    BalanceType = balanceGold,
                    BalanceHistoryType = BalanceHistoryTypes.CategoryWinner,
                    UserId = item.Secondary,
                    Amount = 2000
                });

                @changeBalancesEvent.AddChangeBalance(new ChangeBalanceModel
                {
                    BalanceType = balanceGold,
                    BalanceHistoryType = BalanceHistoryTypes.CategoryWinner,
                    UserId = item.Third,
                    Amount = 1000
                });
            }

            _eventBus.Publish(@changeBalancesEvent);

            await AddNewContestDate(contestDate.FinishDate);

            decimal totalGold = @changeBalancesEvent.ChangeBalances.Sum(x => x.Amount);

            _logger.LogInformation("{contestDateId} numaralı yarışma kazananlarına toplam {totalGold} altın dağıtıldı.", @event.ContestDateId, totalGold);
        }

        /// <summary>
        /// Yeni yarışma başlatır
        /// </summary>
        /// <param name="currentContestFinishDate">Şuan ki yarışma bitiş tarihi</param>
        /// <returns></returns>
        private async Task AddNewContestDate(DateTime currentContestFinishDate)
        {
            DateTime newContestDate = currentContestFinishDate.AddMonths(1);

            bool isSuccess = await _contestDateRepository.AddAsync(currentContestFinishDate, newContestDate);
            if (!isSuccess)
            {
                _logger.LogError("Yeni yarışma tarihi eklenemedi acil kontrol ediniz!!");
                return;
            }

            _logger.LogInformation("Yeni yarışma tarihi eklendi. {newContestDate}", newContestDate);
        }

        #endregion Methods
    }
}
