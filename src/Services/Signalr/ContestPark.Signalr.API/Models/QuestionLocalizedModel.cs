using ContestPark.Core.Enums;

namespace ContestPark.Signalr.API.Models
{
    public class QuestionLocalizedModel
    {
        public Languages Language { get; set; }

        public string Question { get; set; }
    }
}
