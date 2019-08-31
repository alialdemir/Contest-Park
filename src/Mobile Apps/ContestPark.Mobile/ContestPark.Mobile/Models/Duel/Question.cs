using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Duel.Quiz;
using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Duel
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; }

        public QuestionTypes QuestionType { get; set; }

        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();

        public List<QuestionLocalizedModel> Questions { get; set; } = new List<QuestionLocalizedModel>();

        public string NextQuestion { get; set; }
    }
}
