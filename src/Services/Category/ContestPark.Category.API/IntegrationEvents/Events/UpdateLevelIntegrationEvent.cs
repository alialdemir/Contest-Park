using ContestPark.EventBus.Events;

namespace ContestPark.Category.API.IntegrationEvents.Events
{
    public class UpdateLevelIntegrationEvent : IntegrationEvent
    {
        public UpdateLevelIntegrationEvent(string founderUserId,
                                           string opponentUserId,
                                           byte founderExp,
                                           byte opponentExp,
                                           short subCategoryId)
        {
            FounderUserId = founderUserId;
            OpponentUserId = opponentUserId;
            FounderExp = founderExp;
            OpponentExp = opponentExp;
            SubCategoryId = subCategoryId;
        }

        public string FounderUserId { get; set; }
        public string OpponentUserId { get; set; }
        public byte FounderExp { get; set; }
        public byte OpponentExp { get; set; }
        public short SubCategoryId { get; set; }
    }
}
