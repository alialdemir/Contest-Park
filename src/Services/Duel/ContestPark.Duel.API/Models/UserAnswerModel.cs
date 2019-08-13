using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class UserAnswerModel
    {
        public int DuelId { get; set; }

        public string UserId { get; set; }
        public Stylish Stylish { get; set; }
        public byte Time { get; set; }
        public int QuestionId { get; set; }
        public bool IsFounder { get; set; }
        public byte Score { get; set; }
    }
}
