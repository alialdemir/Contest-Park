using ContestPark.Admin.API.Enums;
using ContestPark.Core.Enums;
using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.Question
{
    /// <summary>
    /// Json ile soru ekleme için bu sınıflar eklendi
    /// </summary>
    public class QuestionSaveModel
    {
        public List<QuestionLocalized> QuestionLocalized { get; set; }
        public List<AnswerSaveModel> Answers { get; set; }
        public List<Question> Questions { get; set; }
    }

    public class QuestionLocalized
    {
        public Languages Language { get; set; }
        public string Question { get; set; }
    }

    public class Question
    {
        public AnswerTypes AnswerTypes { get; set; }
        public QuestionTypes QuestionType { get; set; }
        public short SubCategoryId { get; set; }
        public string Link { get; set; }
    }

    public class AnswerSaveModel
    {
        public string CorrectStylish { get; set; }

        public string Stylish1 { get; set; }

        public string Stylish2 { get; set; }

        public string Stylish3 { get; set; }

        public Languages Language { get; set; }
    }
}
