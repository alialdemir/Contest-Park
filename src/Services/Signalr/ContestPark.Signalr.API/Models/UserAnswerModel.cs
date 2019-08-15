using ContestPark.Signalr.API.Enums;

namespace ContestPark.Signalr.API.Models
{
    public class UserAnswerModel
    {
        public Stylish Stylish { get; set; }

        public int DuelId { get; set; }

        public string UserId { get; set; }

        public byte Time { get; set; }

        public int QuestionId { get; set; }
    }
}
