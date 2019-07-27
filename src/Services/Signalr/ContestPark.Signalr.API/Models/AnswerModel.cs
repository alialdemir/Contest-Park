using ContestPark.Core.Enums;

namespace ContestPark.Signalr.API.Models
{
    public class AnswerModel
    {
        public string Answers { get; set; }

        public bool IsCorrect { get; set; }

        public Languages Language { get; set; }
    }
}
