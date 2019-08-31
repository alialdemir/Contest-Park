using ContestPark.EventBus.Events;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
{
    public class DuelStartingModelIntegrationEvent : IntegrationEvent
    {
        public DuelStartingModelIntegrationEvent(int duelId,
                                                  string founderCoverPicturePath,
                                                  string founderProfilePicturePath,
                                                  string founderUserId,
                                                  string founderConnectionId,
                                                  string founderFullName,
                                                  string opponentCoverPicturePath,
                                                  string opponentFullName,
                                                  string opponentProfilePicturePath,
                                                  string opponentUserId,
                                                  string opponentConnectionId)
        {
            DuelId = duelId;
            FounderCoverPicturePath = founderCoverPicturePath;
            FounderProfilePicturePath = founderProfilePicturePath;
            FounderUserId = founderUserId;
            FounderConnectionId = founderConnectionId;
            FounderFullName = founderFullName;
            OpponentCoverPicturePath = opponentCoverPicturePath;
            OpponentFullName = opponentFullName;
            OpponentProfilePicturePath = opponentProfilePicturePath;
            OpponentUserId = opponentUserId;
            OpponentConnectionId = opponentConnectionId;
        }

        public int DuelId { get; set; }

        public string FounderCoverPicturePath { get; set; }

        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath { get; set; }

        public string FounderUserId { get; set; }
        public string FounderConnectionId { get; }
        public string OpponentCoverPicturePath { get; set; }

        public string OpponentFullName { get; set; }

        public string OpponentProfilePicturePath { get; set; }

        public string OpponentUserId { get; set; }
        public string OpponentConnectionId { get; }
    }
}
