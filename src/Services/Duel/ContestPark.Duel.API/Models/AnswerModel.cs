using ContestPark.Core.Enums;
using Newtonsoft.Json;

namespace ContestPark.Duel.API.Models
{
    public class AnswerModel
    {
        public string Answers { get; set; }

        public Languages Language { get; set; }

        [JsonIgnore]
        public bool IsCorrectAnswer { get; set; }
    }
}
