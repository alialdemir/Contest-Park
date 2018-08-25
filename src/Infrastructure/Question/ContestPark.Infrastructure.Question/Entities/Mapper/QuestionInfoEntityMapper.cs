using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Question.Entities.Mapper
{
    public class QuestionInfoEntityMapper : ClassMapper<QuestionInfoEntity>
    {
        public QuestionInfoEntityMapper()
        {
            Table("QuestionInfos");

            AutoMap();
        }
    }
}