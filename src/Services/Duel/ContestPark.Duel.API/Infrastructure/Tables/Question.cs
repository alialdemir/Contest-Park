using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("Questions")]
    public class Question : EntityBase
    {
        [Key]
        public int QuestionId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; } = AnswerTypes.Text;

        public QuestionTypes QuestionType { get; set; } = QuestionTypes.Text;

        public bool IsActive { get; set; } = true;

        public short SubCategoryId { get; set; }
    }
}
