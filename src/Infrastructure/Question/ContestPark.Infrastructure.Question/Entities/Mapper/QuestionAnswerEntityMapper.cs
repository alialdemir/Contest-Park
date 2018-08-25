using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Question.Entities.Mapper
{
    public class QuestionAnswerEntityMapper : ClassMapper<QuestionAnswerEntity>
    {
        public QuestionAnswerEntityMapper()
        {
            Table("QuestionAnswers");

            AutoMap();
        }
    }
}