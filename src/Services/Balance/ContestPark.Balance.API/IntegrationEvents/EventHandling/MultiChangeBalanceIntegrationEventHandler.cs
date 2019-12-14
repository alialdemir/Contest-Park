using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.IntegrationEvents.EventHandling
{
    public class MultiChangeBalanceIntegrationEventHandler : IIntegrationEventHandler<MultiChangeBalanceIntegrationEvent>

    {
        private readonly ILogger<MultiChangeBalanceIntegrationEventHandler> _logger;
        private readonly IBalanceRepository _balanceRepository;

        public MultiChangeBalanceIntegrationEventHandler(ILogger<MultiChangeBalanceIntegrationEventHandler> logger,
                                                         IBalanceRepository balanceRepository)
        {
            _logger = logger;
            _balanceRepository = balanceRepository;
        }

        /// <summary>
        /// Servislerden gelen bakiye değiştirme işlemlerini handler eder
        /// Amount değeri negatif ise bakiye düşer pozitif ise bakiye ekler
        /// </summary>
        /// <param name="event">Bakiye bilgileri</param>
        public async Task Handle(MultiChangeBalanceIntegrationEvent @event)
        {
            // TODO: for tek seferde database de update etmeli

            foreach (var balanceInfo in @event.ChangeBalances)
            {
                _logger.LogInformation("Bakiye değiştirme işlemi handler edildi... {UserId} {Amount} {BalanceType} {BalanceHistoryType}",
                                   balanceInfo.UserId,
                                   balanceInfo.Amount,
                                   balanceInfo.BalanceType,
                                   balanceInfo.BalanceHistoryType);

                if (balanceInfo.UserId.EndsWith("-bot"))
                    continue;

                // TODO: işlem başarısız olursa rabbitmq eventi tekrar tetiklemeli
                //////bool isSuccess = await _balanceRepository.UpdateBalanceAsync(balanceInfo);
                //////if (!isSuccess)
                //////{
                //////    _logger.LogError("Bakiye değiştirme işlemi başarısız oldu... {UserId} {Amount} {BalanceType} {BalanceHistoryType}",
                //////                           balanceInfo.UserId,
                //////                           balanceInfo.Amount,
                //////                           balanceInfo.BalanceType,
                //////                           balanceInfo.BalanceHistoryType);
                //////    return;
                //////}

                _logger.LogInformation("Bakiye değiştirme işlemi başarılı oldu... {UserId} {Amount} {BalanceType} {BalanceHistoryType}",
                                       balanceInfo.UserId,
                                       balanceInfo.Amount,
                                       balanceInfo.BalanceType,
                                       balanceInfo.BalanceHistoryType);
            }
        }
    }
}
