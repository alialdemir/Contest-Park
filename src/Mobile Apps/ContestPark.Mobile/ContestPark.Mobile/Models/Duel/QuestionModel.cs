using ContestPark.Mobile.Models.Duel.Quiz;

namespace ContestPark.Mobile.Models.Duel
{
    public class QuestionModel : SelectedBetModel
    {
        public DuelCreated DuelCreated { get; set; }
        public DuelStartingModel DuelStarting { get; set; }
    }
}
