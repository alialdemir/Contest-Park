using ContestPark.EventBus.Events;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.IntegrationEvents
{
    public interface IIdentityIntegrationEventService
    {
        Task SaveEventAndApplicationContextChangesAsync(IntegrationEvent evt);

        Task PublishThroughEventBusAsync(IntegrationEvent evt);
        void NewPostAdd(Events.NewPostAddedIntegrationEvent newPostAddedIntegrationEvent);
    }
}