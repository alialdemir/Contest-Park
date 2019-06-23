using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.IntegrationEvents.EventHandling
{
    public class SendMessageWithSignalrIntegrationEventHandler : IIntegrationEventHandler<SendMessageWithSignalrIntegrationEvent>
    {
        #region Private Variables

        private readonly ILogger<SendMessageWithSignalrIntegrationEventHandler> _logger;
        private readonly IHubContext<ContestParkHub> _hubContext;

        #endregion Private Variables

        #region Constructor

        public SendMessageWithSignalrIntegrationEventHandler(ILogger<SendMessageWithSignalrIntegrationEventHandler> logger,
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
        public async Task Handle(SendMessageWithSignalrIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                                    .Group(@event.AuthorUserId)
                                    .SendAsync("SendMessage", new
                                    {
                                        @event.AuthorUserId,
                                        @event.ConversationId,
                                        @event.Text,
                                    });
            }
        }

        #endregion Methods
    }
}