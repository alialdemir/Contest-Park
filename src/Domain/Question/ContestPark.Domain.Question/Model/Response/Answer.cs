using Newtonsoft.Json;

namespace ContestPark.Domain.Question.Model.Response
{
    public class Answer
    {
        [JsonIgnore]
        public byte Language { get; set; }

        public bool IsCorrect { get; set; }

        public string Answers { get; set; }
    }
}