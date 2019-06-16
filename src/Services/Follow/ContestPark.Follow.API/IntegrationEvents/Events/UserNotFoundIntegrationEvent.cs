using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Follow.API.IntegrationEvents.Events
{
    public class UserNotFoundIntegrationEvent : IntegrationEvent
    {
        public IEnumerable<string> NotFoundUserIds { get; private set; }

        public UserNotFoundIntegrationEvent(IEnumerable<string> notFoundUserIds)
        {
            NotFoundUserIds = notFoundUserIds;
        }
    }
}