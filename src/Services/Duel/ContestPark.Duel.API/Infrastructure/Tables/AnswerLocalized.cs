using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("AnswerLocalizeds")]
    public class AnswerLocalized : EntityBaseEffaceable
    {
        [Key]
        public int AnswerLocalizedId { get; set; }

        public string CorrectStylish { get; set; }

        public string Stylish1 { get; set; }

        public string Stylish2 { get; set; }

        public string Stylish3 { get; set; }

        public Languages Language { get; set; }

        public int QuestionId { get; set; }
    }
}
