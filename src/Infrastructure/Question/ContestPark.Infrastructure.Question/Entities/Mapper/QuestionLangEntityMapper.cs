using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Question.Entities.Mapper
{
    public class QuestionLangEntityMapper : ClassMapper<QuestionLangEntity>
    {
        public QuestionLangEntityMapper()
        {
            Table("QuestionLangs");

            AutoMap();
        }
    }
}