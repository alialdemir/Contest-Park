using ContestPark.EventBus.Abstractions;
using ContestPark.Signalr.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading.Tasks;

namespace ContestPark.Signalr.API.IntegrationEvents.EventHandling
{
    public class InviteDuelIntegrationEventHandler : IIntegrationEventHandler<InviteDuelIntegrationEvent>
    {
        #region Private Variables

        private readonly ILogger<InviteDuelIntegrationEventHandler> _logger;
        private readonly IHubContext<ContestParkHub> _hubContext;

        #endregion Private Variables

        #region Constructor

        public InviteDuelIntegrationEventHandler(ILogger<InviteDuelIntegrationEventHandler> logger,
                                                 IHubContext<ContestParkHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        #endregion Constructor

        #region Methods

        public async Task Handle(InviteDuelIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                await _hubContext.Clients
                                    .Group(@event.OpponentUserId)
                                    .SendAsync("InviteDuel", new
                                    {
                                        @event.OpponentUserId,
                                        @event.FounderUserId,
                                        @event.FounderConnectionId,
                                        @event.FounderProfilePicturePath,
                                        @event.FounderFullname,
                                        @event.SubCategoryName,
                                        @event.SubCategoryPicture,
                                        @event.BalanceType,
                                        @event.IsOpponentOpenSubCategory,
                                        @event.Bet,
                                        @event.SubCategoryId
                                    });
            }
        }

        #endregion Methods
    }
}
