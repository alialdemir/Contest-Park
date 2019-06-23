using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.IntegrationEvents.EventHandling
{
    public class SendErrorMessageWithSignalrIntegrationEventHandler : IIntegrationEventHandler<SendErrorMessageWithSignalrIntegrationEvent>
    {
        #region Private Variables

        private readonly ILogger<SendErrorMessageWithSignalrIntegrationEventHandler> _logger;
        private readonly IHubContext<ContestParkHub> _hubContext;

        #endregion Private Variables

        #region Constructor

        public SendErrorMessageWithSignalrIntegrationEventHandler(ILogger<SendErrorMessageWithSignalrIntegrationEventHandler> logger,
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
        public async Task Handle(SendErrorMessageWithSignalrIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                                    .Group(@event.UserId)
                                    .SendAsync("SendErrorMessage", new
                                    {
                                        @event.Message
                                    });
            }
        }

        #endregion Methods
    }
}