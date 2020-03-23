using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
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
        public Task Handle(DuelStartingModelIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                string duelGroupName = GetDuelGroupName(@event.DuelId);

                // Eğer bot değilse tek seferde gönderebilemk için İki kullanıcıyı duello id ile bir gruba aldık
                if (!@event.FounderUserId.EndsWith("-bot"))
                {
                    _hubContext.Groups.AddToGroupAsync(@event.FounderConnectionId, duelGroupName);
                }

                // Eğer bot değilse tek seferde gönderebilemk için İki kullanıcıyı duello id ile bir gruba aldık
                if (!@event.OpponentUserId.EndsWith("-bot"))
                {
                    _hubContext.Groups.AddToGroupAsync(@event.OpponentConnectionId, duelGroupName);
                }

                _hubContext.Clients
                                   .Group(duelGroupName)
                                   .SendAsync("DuelStarting", @event);

                _logger.LogInformation(
                    "----- Handling integration event: {IntegrationEventId} at {AppName}",
                    @event.Id,
                    Program.AppName,
                    @event.DuelId,
                    @event.FounderCoverPicturePath,
                    @event.FounderProfilePicturePath,
                    @event.FounderUserId,
                    @event.FounderConnectionId,
                    @event.FounderFullName,
                    @event.OpponentCoverPicturePath,
                    @event.OpponentFullName,
                    @event.OpponentProfilePicturePath,
                    @event.OpponentUserId,
                    @event.OpponentConnectionId);

                return Task.CompletedTask;
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
