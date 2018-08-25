using ContestPark.Core.Model;
using System;

namespace ContestPark.Infrastructure.Question.Entities
{
    public class AskedQuestionEntity : EntityBase
    {
        public int AskedQuestionId { get; set; }

        public int QuestionInfoId { get; set; }

        public string UserId { get; set; }

        public Int16 SubCategoryId { get; set; }
    }
}