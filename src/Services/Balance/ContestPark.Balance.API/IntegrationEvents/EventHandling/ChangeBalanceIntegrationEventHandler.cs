using ContestPark.Balance.API.Infrastructure.Repositories.Balance;
using ContestPark.Balance.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.IntegrationEvents.EventHandling
{
    public class ChangeBalanceIntegrationEventHandler : IIntegrationEventHandler<ChangeBalanceIntegrationEvent>

    {
        private readonly ILogger<ChangeBalanceIntegrationEventHandler> _logger;
        private readonly IBalanceRepository _balanceRepository;

        public ChangeBalanceIntegrationEventHandler(ILogger<ChangeBalanceIntegrationEventHandler> logger,
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
        public async Task Handle(ChangeBalanceIntegrationEvent @event)
        {
            _logger.LogInformation("Bakiye değiştirme işlemi handler edildi... {UserId} {Amount} {BalanceType} {BalanceHistoryType}",
                                   @event.UserId,
                                   @event.Amount,
                                   @event.BalanceType,
                                   @event.BalanceHistoryType);

            if (@event.UserId.EndsWith("-bot"))
                return;

            // TODO: işlem başarısız olursa rabbitmq eventi tekrar tetiklemeli
            bool isSuccess = await _balanceRepository.UpdateBalanceAsync(new Models.ChangeBalanceModel
            {
                Amount = @event.Amount,
                BalanceHistoryType = @event.BalanceHistoryType,
                BalanceType = @event.BalanceType,
                UserId = @event.UserId
            });
            if (!isSuccess)
            {
                _logger.LogError("Bakiye değiştirme işlemi başarısız oldu... {UserId} {Amount} {BalanceType} {BalanceHistoryType}",
                                       @event.UserId,
                                       @event.Amount,
                                       @event.BalanceType,
                                       @event.BalanceHistoryType);
                return;
            }

            _logger.LogInformation("Bakiye değiştirme işlemi başarılı oldu... {UserId} {Amount} {BalanceType} {BalanceHistoryType}",
                                   @event.UserId,
                                   @event.Amount,
                                   @event.BalanceType,
                                   @event.BalanceHistoryType);
        }
    }
}
