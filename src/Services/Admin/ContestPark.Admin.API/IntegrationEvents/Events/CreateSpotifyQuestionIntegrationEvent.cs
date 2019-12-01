using ContestPark.Admin.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Admin.API.IntegrationEvents.Events
{
    public class CreateSpotifyQuestionIntegrationEvent : IntegrationEvent
    {
        public CreateSpotifyQuestionIntegrationEvent(short subCategoryId, string spotifyId, SpotifyQuestionTypes spotifyQuestionType)
        {
            SubCategoryId = subCategoryId;
            SpotifyId = spotifyId;
            SpotifyQuestionType = spotifyQuestionType;
        }

        public short SubCategoryId { get; }
        public string SpotifyId { get; }
        public SpotifyQuestionTypes SpotifyQuestionType { get; }
    }
}
