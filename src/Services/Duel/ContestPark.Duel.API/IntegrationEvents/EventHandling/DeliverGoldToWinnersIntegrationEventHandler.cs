using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository;
using ContestPark.Duel.API.IntegrationEvents.Events;
using ContestPark.Duel.API.Models;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.IntegrationEvents.EventHandling
{
    public class DeliverGoldToWinnersIntegrationEventHandler : IIntegrationEventHandler<DeliverGoldToWinnersIntegrationEvent>
    {
        private readonly IScoreRankingRepository _scoreRankingRepository;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly ILogger<DeliverGoldToWinnersIntegrationEventHandler> _logger;
        private readonly IEventBus _eventBus;

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

        public Task Handle(DeliverGoldToWinnersIntegrationEvent @event)
        {
            _logger.LogInformation("{contestDateId} numaralı yarışma kazananlarına altınlar dağıtılıyor.", @event.ContestDateId);

            //ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();
            //if (contestDate.FinishDate > DateTime.Now)
            //{
            //    _logger.LogError("Süresi dolmamış yarışmanın ödülleri dağıtılmaya çalışıldı.");

            //    return Task.CompletedTask;
            //}

            BalanceTypes balanceGold = BalanceTypes.Gold;

            IEnumerable<WinnersModel> winners = _scoreRankingRepository.Winners(@event.ContestDateId,
                                                                                balanceGold);

            if (winners == null || !winners.Any())
            {
                _logger.LogWarning("{contestDateId} numaralı yarışmada kazanan bulunamadı.", @event.ContestDateId);

                return Task.CompletedTask;
            }

            List<ChangeBalanceModel> changeBalances = new List<ChangeBalanceModel>();

            foreach (var item in winners)
            {
                ChangeBalanceModel changeBalance = new ChangeBalanceModel
                {
                    BalanceType = balanceGold,
                    BalanceHistoryType = BalanceHistoryTypes.CategoryWinner,
                    UserId = item.Premier,
                    Amount = 3000
                };
                changeBalances.Add(changeBalance);

                changeBalance.Amount = 2000;
                changeBalance.UserId = item.Secondary;
                changeBalances.Add(changeBalance);

                changeBalance.Amount = 1000;
                changeBalance.UserId = item.Third;
                changeBalances.Add(changeBalance);
            }

            var json = JsonConvert.SerializeObject(changeBalances);
            _logger.LogInformation("kazananlar json: {json}", json);

            var @changeBalancesEvent = new MultiChangeBalanceIntegrationEvent(changeBalances);
            _eventBus.Publish(@changeBalancesEvent);

            _logger.LogInformation("{contestDateId} numaralı yarışma kazananlarına altınlar dağıtıldı.", @event.ContestDateId);

            return Task.CompletedTask;
        }
    }
}
