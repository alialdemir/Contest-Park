namespace ContestPark.Duel.API.Infrastructure.Repositories.AskedQuestion
{
    public interface IAskedQuestionRepository
    {
        System.Threading.Tasks.Task<bool> Delete(short subCategoryId, params string[] userIds);
        void Insert(short subCategoryId, int[] questions, params string[] userIds);
    }
}
