using ContestPark.Admin.API.Model.Question;
using ContestPark.EventBus.Events;
using System.Collections.Generic;

namespace ContestPark.Admin.API.IntegrationEvents.Events
{
    public class CreateQuestionIntegrationEvent : IntegrationEvent
    {
        public CreateQuestionIntegrationEvent(List<QuestionSaveModel> questions)
        {
            Questions = questions;
        }

        public List<QuestionSaveModel> Questions { get; }
    }
}
