using ContestPark.EventBus.Events;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class UserInfoChangedIntegrationEvent : IntegrationEvent
    {
        public string NewFullName { get; private set; }

        public string NewUserName { get; private set; }

        public string OldFullName { get; private set; }

        public string OldUserName { get; private set; }

        public string UserId { get; private set; }

        public UserInfoChangedIntegrationEvent(string userId,
                                               string newFullName,
                                               string newUserName,
                                               string oldFullName,
                                               string oldUserName)
        {
            UserId = userId;
            NewFullName = newFullName;
            NewUserName = newUserName;

            OldFullName = oldFullName;
            OldUserName = oldUserName;
        }
    }
}