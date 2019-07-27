using ContestPark.Core.Enums;
using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class QuestionTableModel
    {
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; }

        public QuestionTypes QuestionType { get; set; }

        public string Question { get; set; }

        public Languages Language { get; set; }

        public string CorrectStylish { get; set; }

        public string Stylish1 { get; set; }

        public string Stylish2 { get; set; }

        public string Stylish3 { get; set; }
    }
}
