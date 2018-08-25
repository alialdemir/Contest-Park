using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Question.Entities.Mapper
{
    public class AskedQuestionEntityMapper : ClassMapper<AskedQuestionEntity>
    {
        public AskedQuestionEntityMapper()
        {
            Table("AskedQuestions");

            AutoMap();
        }
    }
}