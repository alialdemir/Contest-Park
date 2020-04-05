using ContestPark.EventBus.Events;
using ContestPark.Signalr.API.Enums;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
{
    public class NextQuestionIntegrationEvent : IntegrationEvent
    {
        public NextQuestionIntegrationEvent(int duelId,
                                             string founderUserId,
                                             string opponentUserId,
                                             Stylish founderStylish,
                                             Stylish opponentStylish,
                                             Stylish correctStylish,
                                             byte founderScore,
                                             byte opponentScore,
                                             byte round,
                                             bool isGameEnd)
        {
            DuelId = duelId;
            FounderStylish = founderStylish;
            OpponentStylish = opponentStylish;
            CorrectStylish = correctStylish;
            FounderScore = founderScore;
            OpponentScore = opponentScore;
            Round = round;
            IsGameEnd = isGameEnd;
            FounderUserId = founderUserId;
            OpponentUserId = opponentUserId;
        }

        public int DuelId { get; }
        public Stylish FounderStylish { get; set; }

        public Stylish OpponentStylish { get; set; }

        public Stylish CorrectStylish { get; set; }

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }
        public byte Round { get; }
        public bool IsGameEnd { get; set; }
        public string FounderUserId { get; }
        public string OpponentUserId { get; }
    }
}
