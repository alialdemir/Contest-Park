using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.IntegrationEvents.EventHandling
{
    public class NextQuestionIntegrationEventHandler : IIntegrationEventHandler<NextQuestionIntegrationEvent>
    {
        #region Private Variables

        private readonly ILogger<NextQuestionIntegrationEventHandler> _logger;
        private readonly IHubContext<ContestParkHub> _hubContext;

        #endregion Private Variables

        #region Constructor

        public NextQuestionIntegrationEventHandler(ILogger<NextQuestionIntegrationEventHandler> logger,
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
        public async Task Handle(NextQuestionIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                string duelGroupName = GetDuelGroupName(@event.DuelId);

                if (@event.FounderUserId.EndsWith("-bot") || @event.OpponentUserId.EndsWith("-bot"))
                {
                    string realUserId = @event.FounderUserId.EndsWith("-bot")
                        ? @event.OpponentUserId
                        : @event.FounderUserId;

                    duelGroupName = realUserId;
                }

                await _hubContext.Clients
                                 .Group(duelGroupName)
                                 .SendAsync("NextQuestion", @event);

                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
            }
        }

        /// <summary>
        /// Duello group adını  verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello grup adı</returns>
        private string GetDuelGroupName(int duelId)
        {
            return $"Duel{duelId}";
        }

        #endregion Methods
    }
}
