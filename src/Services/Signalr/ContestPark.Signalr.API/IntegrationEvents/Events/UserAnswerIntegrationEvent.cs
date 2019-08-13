using ContestPark.EventBus.Events;
using ContestPark.Signalr.API.Enums;

namespace ContestPark.Signalr.API.IntegrationEvents.Events
{
    public class UserAnswerIntegrationEvent : IntegrationEvent
    {
        public UserAnswerIntegrationEvent(int duelId,
                                          string userId,
                                          int questionId,
                                          Stylish stylish,
                                          byte time,
                                          bool isFounder,
                                          byte round)
        {
            DuelId = duelId;
            UserId = userId;
            QuestionId = questionId;
            Stylish = stylish;
            Time = time;
            IsFounder = isFounder;
            Round = round;
        }

        public Stylish Stylish { get; set; }

        public int DuelId { get; set; }

        public bool IsFounder { get; set; }

        public string UserId { get; set; }
        public byte Round { get; set; }

        public byte Time { get; set; }

        public int QuestionId { get; set; }
    }
}
