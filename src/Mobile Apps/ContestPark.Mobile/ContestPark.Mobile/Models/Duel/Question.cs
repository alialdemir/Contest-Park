using ContestPark.Mobile.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Mobile.Models.Duel
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; }

        public QuestionTypes QuestionType { get; set; }

        public int QuestionInfoId { get; set; }

        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();

        public List<QuestionLang> Questions { get; set; } = new List<QuestionLang>();

        public string NextQuestion { get; set; }

        public Question GetQuestionByLanguage(Languages language)
        {
            return new Question
            {
                Link = this.Link,
                QuestionId = this.QuestionId,
                AnswerType = this.AnswerType,
                QuestionType = this.QuestionType,
                QuestionInfoId = this.QuestionInfoId,
                Answers = this.Answers.Where(p => p.Language == language).ToList(),
                NextQuestion = this.Questions.Where(p => p.Language == language).Select(x => x.Question).FirstOrDefault(),
            };
        }
    }
}