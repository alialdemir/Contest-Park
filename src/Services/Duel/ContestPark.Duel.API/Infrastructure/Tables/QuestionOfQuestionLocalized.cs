using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("QuestionOfQuestionLocalizeds")]
    public class QuestionOfQuestionLocalized : EntityBase
    {
        [Key]
        public int QuestionOfQuestionLocalizedId { get; set; }

        public int QuestionId { get; set; }

        public int QuestionLocalizedId { get; set; }
    }
}
