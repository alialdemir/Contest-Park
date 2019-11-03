using ContestPark.Admin.API.Enums;
using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Admin.API.Infrastructure.Tables
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
