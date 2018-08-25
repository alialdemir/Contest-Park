using ContestPark.Core.Enums;

namespace ContestPark.Domain.Question.Model.Response
{
    public class QuestionCreated
    {
        public Question Question { get; set; }

        public string FounderConnectionId { get; set; }

        public string OpponentConnectionId { get; set; }

        public Languages FounderLanguage { get; set; }

        public Languages OpponentLanguage { get; set; }

        public QuestionCreated(
            string founderConnectionId,
            string opponentConnectionId,
            Question question,
            Languages founderLanguage,
            Languages opponentLanguage)
        {
            FounderConnectionId = founderConnectionId;
            OpponentConnectionId = opponentConnectionId;
            Question = question;
            FounderLanguage = founderLanguage;
            OpponentLanguage = opponentLanguage;
        }
    }
}