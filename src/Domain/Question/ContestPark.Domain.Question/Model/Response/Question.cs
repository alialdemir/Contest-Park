using ContestPark.Domain.Question.Enums;
using System.Collections.Generic;

namespace ContestPark.Domain.Question.Model.Response
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; }

        public QuestionTypes QuestionType { get; set; }

        public int QuestionInfoId { get; set; }

        public List<Answer> Answers { get; set; } = new List<Answer>();

        public List<QuestionLang> Questions { get; set; } = new List<QuestionLang>();
    }
}