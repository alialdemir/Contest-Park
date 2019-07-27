using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("QuestionLocalizeds")]
    public class QuestionLocalized : EntityBase
    {
        [Key]
        public int QuestionLocalizedId { get; set; }

        public string Question { get; set; }

        public Languages Language { get; set; }
    }
}
