using System.Collections.Generic;

namespace ContestPark.Duel.API.Models
{
    public class DuelEscapeModel
    {
        public int DuelId { get; set; }

        public string FounderUserId { get; set; }

        public string OpponentUserId { get; set; }

        public List<DuelFinishQuestionModel> Questions { get; set; }
    }
}
