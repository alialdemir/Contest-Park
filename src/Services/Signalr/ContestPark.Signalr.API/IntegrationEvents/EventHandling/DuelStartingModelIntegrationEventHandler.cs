using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.IntegrationEvents.EventHandling
{
    public class DuelStartingModelIntegrationEventHandler : IIntegrationEventHandler<DuelStartingModelIntegrationEvent>
    {
        #region Private Variables

        private readonly ILogger<DuelStartingModelIntegrationEventHandler> _logger;
        private readonly IHubContext<ContestParkHub> _hubContext;

        #endregion Private Variables

        #region Constructor

        public DuelStartingModelIntegrationEventHandler(ILogger<DuelStartingModelIntegrationEventHandler> logger,
                                                                  IHubContext<ContestParkHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Server tarafından client'lere hata mesajı gibi mesajları gönderir
        /// </summary>
        /// <param name="event">Mesaj bilgisi</param>
        public async Task Handle(DuelStartingModelIntegrationEvent @event)
        {
            Debug.WriteLine("@event.FounderProfilePicturePath" + @event.FounderProfilePicturePath);
            Debug.WriteLine("@@event.OpponentProfilePicturePath" + @event.OpponentProfilePicturePath);
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                string duelGroupName = GetDuelGroupName(@event.DuelId);

                // Eğer bot değilse tek seferde gönderebilemk için İki kullanıcıyı duello id ile bir gruba aldık
                if (!@event.FounderUserId.EndsWith("bot"))
                {
                    await _hubContext.Groups.AddToGroupAsync(@event.FounderConnectionId, duelGroupName);
                }

                // Eğer bot değilse tek seferde gönderebilemk için İki kullanıcıyı duello id ile bir gruba aldık
                if (!@event.OpponentUserId.EndsWith("bot"))
                {
                    await _hubContext.Groups.AddToGroupAsync(@event.OpponentConnectionId, duelGroupName);
                }

                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                                    .Group(duelGroupName)
                                    .SendAsync("DuelStarting", @event);
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
