using ContestPark.Admin.API.Model.Question;
using ContestPark.EventBus.Events;

namespace ContestPark.Admin.API.IntegrationEvents.Events
{
    public class QuestionConfigIntegrationEvent : IntegrationEvent
    {
        public QuestionConfigIntegrationEvent(QuestionConfigModel questionConfig)
        {
            QuestionConfig = questionConfig;
        }

        public QuestionConfigModel QuestionConfig { get; }
    }
}
}
