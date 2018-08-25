using ContestPark.Core.Enums;
using ContestPark.Core.Model;

namespace ContestPark.Infrastructure.Question.Entities
{
    public class QuestionAnswerEntity : EntityBase
    {
        public int QuestionAnswerId { get; set; }

        public string Answer { get; set; }

        public bool IsCorrect { get; set; } = false;

        public Languages LanguageId { get; set; }

        public int QuestionInfoId { get; set; }

        public int QuestionId { get; set; }
    }
}