using ContestPark.EventBus.Events;

namespace ContestPark.Post.API.IntegrationEvents.Events
{
    public class NewUserRegisterIntegrationEvent : IntegrationEvent
    {
        public string FullName { get; private set; }

        public string UserName { get; private set; }

        public string ProfilePicturePath { get; private set; }

        public string UserId { get; set; }

        public NewUserRegisterIntegrationEvent(string userId, string fullName, string userName, string profilePicturePath)
        {
            UserId = userId;
            FullName = fullName;
            UserName = userName;
            ProfilePicturePath = profilePicturePath;
        }
    }
}
