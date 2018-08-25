using ContestPark.Core.Model;
using ContestPark.Domain.Duel.Enums;

namespace ContestPark.Infrastructure.Duel.Entities
{
    public class DuelInfoEntity : EntityBase
    {
        public int DuelInfoId { get; set; }

        public int DuelId { get; set; }

        public int QuestionInfoId { get; set; }

        public Stylish FounderAnswer { get; set; } = Stylish.NotSeeQuestion; //Duelloyu başlatanın verdiği cevap

        public Stylish OpponentAnswer { get; set; } = Stylish.NotSeeQuestion;//Rakibin verdiği cevap

        public Stylish CorrectAnswer { get; set; } = Stylish.NotSeeQuestion;// Doğru cevap yeri

        public byte FounderTime { get; set; }//Kurucunun cevap verdiği süre

        public byte OpponentTime { get; set; }//Rakibin cevap verdiği süre

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }
    }
}