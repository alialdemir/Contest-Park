using ContestPark.EventBus.Abstractions;
using ContestPark.EventBus.Events;
using ContestPark.EventBus.IntegrationEventLogEF.Services;
using ContestPark.EventBus.IntegrationEventLogEF.Utilities;
using ContestPark.Identity.API.Data;
using ContestPark.Identity.API.IntegrationEvents.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents
{
    public class IdentityIntegrationEventService : IIdentityIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<IdentityIntegrationEventService> _logger;

        public IdentityIntegrationEventService(
            ILogger<IdentityIntegrationEventService> logger,
            IEventBus eventBus,
            ApplicationDbContext applicationDbContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_applicationDbContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", evt.Id, Program.AppName, evt);

                await _eventLogService.MarkEventAsInProgressAsync(evt.Id);
                _eventBus.Publish(evt);
                await _eventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", evt.Id, Program.AppName, evt);
                await _eventLogService.MarkEventAsFailedAsync(evt.Id);
            }
        }

        public async Task SaveEventAndApplicationContextChangesAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", evt.Id);

            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            await ResilientTransaction.New(_applicationDbContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _applicationDbContext.SaveChangesAsync();
                await _eventLogService.SaveEventAsync(evt, _applicationDbContext.Database.CurrentTransaction);
            });
        }

        /// <summary>
        /// Post ekle eventi publish eder
        /// </summary>
        /// <param name="newPostAddedIntegrationEvent">Post detayı</param>
        public void NewPostAdd(NewPostAddedIntegrationEvent @event)
        {
            _eventBus.Publish(@event);
        }
    }
}