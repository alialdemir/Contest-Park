using ContestPark.Duel.API.Enums;
using System.Collections.Generic;

namespace ContestPark.Duel.API.Models
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; }

        public QuestionTypes QuestionType { get; set; }

        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();

        public List<QuestionLocalizedModel> Questions { get; set; } = new List<QuestionLocalizedModel>();
    }
}
