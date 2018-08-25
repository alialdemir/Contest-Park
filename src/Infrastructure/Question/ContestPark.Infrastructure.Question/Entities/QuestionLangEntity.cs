using ContestPark.Core.Enums;
using ContestPark.Core.Model;

namespace ContestPark.Infrastructure.Question.Entities
{
    public class QuestionLangEntity : EntityBase
    {
        public int QuestionLangId { get; set; }

        public int QuestionId { get; set; }

        public string Question { get; set; }

        public Languages LanguageId { get; set; }
    }
}