using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Question.Entities.Mapper
{
    public class QuestionEntityMapper : ClassMapper<QuestionEntity>
    {
        public QuestionEntityMapper()
        {
            Table("Questions");

            AutoMap();
        }
    }
}