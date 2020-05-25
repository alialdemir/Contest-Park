using ContestPark.EventBus.Abstractions;
using ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission;
using ContestPark.Mission.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Mission.API.IntegrationEvents.EventHandling
{
    public class CheckMissionIntegrationEventHandler : IIntegrationEventHandler<CheckMissionIntegrationEvent>
    {
        #region Private variables

        private readonly ICompletedMissionRepository _completedMissionRepository;
        private readonly ILogger<CheckMissionIntegrationEventHandler> _logger;

        #endregion Private variables

        #region Constructor

        public CheckMissionIntegrationEventHandler(ICompletedMissionRepository completedMissionRepository,
                                                   ILogger<CheckMissionIntegrationEventHandler> logger)
        {
            _completedMissionRepository = completedMissionRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Görev id göre görev tamamlanmış mı kontrol eder
        /// </summary>
        public Task Handle(CheckMissionIntegrationEvent @event)
        {
            _completedMissionRepository.IsMissionCompleted(@event.UserId, @event.MissionId);

            return Task.CompletedTask;
        }

        #endregion Methods
    }
}
