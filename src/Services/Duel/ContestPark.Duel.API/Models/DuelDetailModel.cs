using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class DuelDetailModel
    {
        public int DuelId { get; set; }

        public int QuestionId { get; set; }

        public Stylish CorrectAnswer { get; set; }
    }
}
