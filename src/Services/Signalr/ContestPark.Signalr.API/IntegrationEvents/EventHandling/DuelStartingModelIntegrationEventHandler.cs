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
        private readonly IEventBus _eventBus;
        private readonly IHubContext<ContestParkHub> _hubContext;
        private byte COUNT = 0;

        #endregion Private Variables

        #region Constructor

        public DuelStartingModelIntegrationEventHandler(ILogger<DuelStartingModelIntegrationEventHandler> logger,
                                                        IEventBus eventBus,
                                                        IHubContext<ContestParkHub> hubContext)
        {
            _logger = logger;
            _eventBus = eventBus;
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
            try
            {
                using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
                {
                    string duelGroupName = GetDuelGroupName(@event.DuelId);

                    // Eğer bot değilse tek seferde gönderebilemk için İki kullanıcıyı duello id ile bir gruba aldık
                    if (!@event.FounderUserId.EndsWith("-bot") && !string.IsNullOrEmpty(@event.FounderConnectionId))
                    {
                        await _hubContext.Groups.AddToGroupAsync(@event.FounderConnectionId, duelGroupName);
                    }

                    // Eğer bot değilse tek seferde gönderebilemk için İki kullanıcıyı duello id ile bir gruba aldık
                    if (!@event.OpponentUserId.EndsWith("-bot") && !string.IsNullOrEmpty(@event.OpponentConnectionId))
                    {
                        await _hubContext.Groups.AddToGroupAsync(@event.OpponentConnectionId, duelGroupName);
                    }

                    await _hubContext.Clients
                                         .Group(duelGroupName)
                                         .SendAsync("DuelStarting", @event);

                    _logger.LogInformation(
                        "----- Handling integration event: {IntegrationEventId} at {AppName} {DuelId} {FounderUserId} {OpponentUserId}",
                        @event.Id,
                        Program.AppName,
                        @event.DuelId,
                        @event.FounderUserId,
                        @event.OpponentUserId);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Düello başlatılırken exception oluştu. {message} ", ex.Message);

                COUNT += 1;

                if (COUNT >= 3)// 3 kere denesin daha fazla hata olursa düelloyu iptal etsin
                {
                    var @eventDuelEscape = new DuelEscapeIntegrationEvent(@event.DuelId,
                                                                          @event.FounderUserId,
                                                                          isDuelCancel: true);

                    _eventBus.Publish(@eventDuelEscape);

                    return;
                }

                await Handle(@event);
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
