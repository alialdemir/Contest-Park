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
        public Task Handle(ChangeBalanceIntegrationEvent @event)
        {
            // TODO: işlem başarısız olursa rabbitmq eventi tekrar tetiklemeli
            _balanceRepository.ChangeBalanceByUserId(new Models.ChangeBalanceModel
            {
                Amount = @event.Amount,
                BalanceHistoryType = @event.BalanceHistoryType,
                BalanceType = @event.BalanceType,
                UserId = @event.UserId
            });

            return Task.CompletedTask;
        }
    }
}