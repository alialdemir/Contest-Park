using ContestPark.Core.Model;
using ContestPark.Domain.Question.Enums;
using System;

namespace ContestPark.Infrastructure.Question.Entities
{
    public class QuestionInfoEntity : EntityBase
    {
        public int QuestionInfoId { get; set; }

        public string Link { get; set; }

        public AnswerTypes AnswerType { get; set; } = AnswerTypes.Text;

        public QuestionTypes QuestionType { get; set; } = QuestionTypes.Text;

        public bool IsActive { get; set; } = true;

        public Int16 SubCategoryId { get; set; }

        public int QuestionId { get; set; }
    }
}