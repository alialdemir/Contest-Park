using ContestPark.Admin.API.Enums;

namespace ContestPark.Admin.API.Model.Question
{
    public class QuestionConfigModel
    {
        public string Json { get; set; }

        public short SubCategoryId { get; set; }

        public QuestionTypes QuestionType { get; set; }
        public AnswerTypes AnswerType { get; set; }

        public string Question { get; set; }

        public string AnswerKey { get; set; }

        public string LinkKey { get; set; }

        public ConvertLanguageTypes QuestionLanguage { get; set; }
        public ConvertLanguageTypes AnswerLanguage { get; set; }
    }
}
