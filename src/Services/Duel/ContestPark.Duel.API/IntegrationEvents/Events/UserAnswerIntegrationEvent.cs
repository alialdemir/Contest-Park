using ContestPark.Duel.API.Enums;
using ContestPark.EventBus.Events;

namespace ContestPark.Duel.API.IntegrationEvents.Events
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

        public string UserId { get; set; }
        public byte Round { get; set; }

        public byte Time { get; set; }
        public bool IsFounder { get; }
        public int QuestionId { get; set; }
    }
}
