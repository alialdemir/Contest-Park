namespace ContestPark.Admin.API.Infrastructure.Repositories.Question
{
    public interface IQuestionRepository
    {
        System.Threading.Tasks.Task<int> Insert(Tables.Question question);
    }
}
