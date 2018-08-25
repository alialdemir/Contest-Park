using ContestPark.Core.Enums;
using ContestPark.Domain.Question.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Domain.Question.Model.Response
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; }

        public QuestionTypes QuestionType { get; set; }

        public string NextQuestion { get; set; }

        public int QuestionInfoId { get; set; }

        public List<Answer> Answers { get; set; } = new List<Answer>();

        [JsonIgnore]
        public List<QuestionLang> Questions { get; set; } = new List<QuestionLang>();

        public Question GetQuestionByLanguage(Languages language)
        {
            byte languageId = (byte)language;

            return new Question
            {
                Link = this.Link,
                QuestionId = this.QuestionId,
                AnswerType = this.AnswerType,
                QuestionType = this.QuestionType,
                QuestionInfoId = this.QuestionInfoId,
                Answers = this.Answers.Where(p => p.Language == languageId).ToList(),
                NextQuestion = this.Questions.Where(p => p.Language == languageId).Select(x => x.Question).FirstOrDefault(),
            };
        }
    }
}