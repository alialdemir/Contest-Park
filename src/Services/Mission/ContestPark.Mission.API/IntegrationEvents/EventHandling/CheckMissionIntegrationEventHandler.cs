using ContestPark.EventBus.Abstractions;
using ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission;
using ContestPark.Mission.API.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace ContestPark.Mission.API.IntegrationEvents.EventHandling
{
    public class CheckMissionIntegrationEventHandler : IIntegrationEventHandler<CheckMissionIntegrationEvent>
    {
        #region Private variables

        private readonly ICompletedMissionRepository _completedMissionRepository;

        #endregion Private variables

        #region Constructor

        public CheckMissionIntegrationEventHandler(ICompletedMissionRepository completedMissionRepository)
        {
            _completedMissionRepository = completedMissionRepository;
        }

        #endregion Constructor

        #region Methods

        public Task Handle(CheckMissionIntegrationEvent @event)
        {
            bool

            return Task.CompletedTask;
        }

        #endregion Methods
    }
}
