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
                                          byte time)
        {
            DuelId = duelId;
            UserId = userId;
            QuestionId = questionId;
            Stylish = stylish;
            Time = time;
        }

        public Stylish Stylish { get; set; }

        public int DuelId { get; set; }

        public string UserId { get; set; }

        public byte Time { get; set; }

        public int QuestionId { get; set; }
    }
}
