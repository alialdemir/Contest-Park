using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("DuelDetails")]
    public class DuelDetail : EntityBase
    {
        [Key]
        public int DuelDetailId { get; set; }

        public int DuelId { get; set; }

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
