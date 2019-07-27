using ContestPark.Duel.API.Enums;

namespace ContestPark.Duel.API.Models
{
    public class DuelFinishQuestionModel
    {
        public int QuestionId { get; set; }

        public Stylish FounderAnswer { get; set; }//Duelloyu başlatanın verdiği cevap

        public Stylish OpponentAnswer { get; set; }//Rakibin verdiği cevap

        public Stylish CorrectAnswer { get; set; }// Doğru cevap yeri

        public byte FounderTime { get; set; }//Kurucunun cevap verdiği süre

        public byte OpponentTime { get; set; }//Rakibin cevap verdiği süre

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }
    }
}
