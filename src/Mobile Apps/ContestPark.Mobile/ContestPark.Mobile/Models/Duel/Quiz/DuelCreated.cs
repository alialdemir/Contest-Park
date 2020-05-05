using System.Collections.Generic;

namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class DuelCreated : DuelStartingModel
    {
        public IEnumerable<QuestionModel> Questions { get; set; }
    }
}
