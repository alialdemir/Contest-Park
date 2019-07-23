using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Duel.API.Infrastructure.Tables
{
    [Table("AskedQuestions")]
    public class AskedQuestion : EntityBase
    {
        [Key]
        public int AskedQuestionId { get; set; }

        public int QuestionId { get; set; }

        public string UserId { get; set; }

        public int SubCategoryId { get; set; }
    }
}
