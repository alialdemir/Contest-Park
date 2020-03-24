using ContestPark.Duel.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Duel.API.Models
{
    public class UserAnswerModel
    {
        public int DuelId { get; set; }

        public int QuestionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte FounderScore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte OpponentScore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Stylish FounderAnswer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Stylish OpponentAnswer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte FounderTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte OpponentTime { get; set; }

        public string FounderUserId { get; set; }
        public string OpponentUserId { get; set; }
        public Stylish CorrectAnswer { get; set; }
        public bool? IsWinStatus { get; set; }
    }
}
