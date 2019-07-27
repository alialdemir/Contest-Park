using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.IntegrationEvents.EventHandling
{
    public class DuelCreatedIntegrationEventHandler : IIntegrationEventHandler<DuelCreatedIntegrationEvent>
    {
        #region Private Variables

        private readonly ILogger<DuelCreatedIntegrationEventHandler> _logger;
        private readonly IHubContext<ContestParkHub> _hubContext;

        #endregion Private Variables

        #region Constructor

        public DuelCreatedIntegrationEventHandler(ILogger<DuelCreatedIntegrationEventHandler> logger,
                                                             IHubContext<ContestParkHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düello başladı bilgisini kullanıcılara gönderir
        /// </summary>
        /// <param name="event">Düello bilgisi</param>
        public async Task Handle(DuelCreatedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                                 .Group(GetDuelGroupName(@event.DuelId))
                                 .SendAsync("DuelCreated", @event);
            }
        }

        /// <summary>
        /// Duello group adını  verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello grup adı</returns>
        private string GetDuelGroupName(int duelId)
        {
            return $"Duel{duelId.ToString()}";
        }

        #endregion Methods
    }
}
