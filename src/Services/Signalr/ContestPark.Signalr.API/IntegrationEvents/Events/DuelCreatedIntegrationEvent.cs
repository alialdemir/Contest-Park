using ContestPark.EventBus.Events;
using ContestPark.Signalr.API.Models;
using System.Collections.Generic;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
{
    public class DuelCreatedIntegrationEvent : IntegrationEvent
    {
        public DuelCreatedIntegrationEvent(int duelId,
                                           IEnumerable<QuestionModel> questions,
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
            Questions = questions;

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

        public IEnumerable<QuestionModel> Questions { get; set; }

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
