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
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                                       @event.Id,
                                       Program.AppName,
                                       @event);

                string duelGroupName = GetDuelGroupName(@event.DuelId);

                if (@event.FounderUserId.EndsWith("-bot") || @event.OpponentUserId.EndsWith("-bot"))
                {
                    // Eğer bot ile oynuyorsa group id kullanıcı id olarak ayarlıyoruz.

                    string realUserId = @event.FounderUserId.EndsWith("-bot")
                        ? @event.OpponentUserId
                        : @event.FounderUserId;

                    duelGroupName = realUserId;
                }
                else if (!@event.FounderUserId.EndsWith("-bot") || !@event.OpponentUserId.EndsWith("-bot"))
                {
                    // Eğer bot değilse tek seferde gönderebilmek için İki kullanıcıyı duello id ile bir gruba aldık
                    if (!@event.FounderUserId.EndsWith("-bot") && !string.IsNullOrEmpty(@event.FounderConnectionId))
                    {
                        await _hubContext.Groups.AddToGroupAsync(@event.FounderConnectionId, duelGroupName);
                    }

                    // Eğer bot değilse tek seferde gönderebilmek için İki kullanıcıyı duello id ile bir gruba aldık
                    if (!@event.OpponentUserId.EndsWith("-bot") && !string.IsNullOrEmpty(@event.OpponentConnectionId))
                    {
                        await _hubContext.Groups.AddToGroupAsync(@event.OpponentConnectionId, duelGroupName);
                    }
                }

                await _hubContext.Clients
                                 .Group(duelGroupName)
                                 .SendAsync("DuelCreated", @event);

                _logger.LogInformation("Düello oluşturuldu: {IntegrationEventId} at {AppName} {DuelId} {FounderUserId} {OpponentUserId}",
                                       @event.Id,
                                       Program.AppName,
                                       @event.DuelId,
                                       @event.FounderUserId,
                                       @event.OpponentUserId);
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
