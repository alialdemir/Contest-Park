using ContestPark.EventBus.Events;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class ProfilePictureChangedIntegrationEvent : IntegrationEvent
    {
        public string NewProfilePicturePath { get; private set; }

        public string OldProfilePicturePath { get; private set; }

        public string UserId { get; private set; }

        public ProfilePictureChangedIntegrationEvent(string userId,
                                                     string newProfilePicturePath,
                                                     string oldProfilePicturePath)
        {
            UserId = userId;

            OldProfilePicturePath = oldProfilePicturePath;
            NewProfilePicturePath = newProfilePicturePath;
        }
    }
}