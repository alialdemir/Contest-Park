using ContestPark.Mobile.Enums;

namespace ContestPark.Mobile.Models.Duel.Quiz
{
    public class UserAnswer
    {
        public Stylish Stylish { get; set; }

        public int DuelId { get; set; }

        public string UserId { get; set; }

        public byte Time { get; set; }

        public int QuestionId { get; set; }
    }
}
