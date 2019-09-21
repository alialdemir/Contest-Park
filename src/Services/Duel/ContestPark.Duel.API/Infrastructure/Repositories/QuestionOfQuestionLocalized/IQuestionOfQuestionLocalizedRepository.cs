namespace ContestPark.Duel.API.Infrastructure.Repositories.QuestionOfQuestionLocalized
{
    public interface IQuestionOfQuestionLocalizedRepository
    {
        System.Threading.Tasks.Task<bool> Insert(Tables.QuestionOfQuestionLocalized questionOfQuestion);
    }
}
