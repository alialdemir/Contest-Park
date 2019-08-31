using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class DuelCreated
    {
        public IEnumerable<QuestionModel> Questions { get; set; }

        public int DuelId { get; set; }
    }
}
