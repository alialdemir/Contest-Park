namespace ContestPark.Duel.API.Infrastructure.Repositories.AnswerLocalized
{
    public interface IAnswerLocalizedRepository
    {
        System.Threading.Tasks.Task<bool> Insert(Tables.AnswerLocalized answerLocalized);
    }
}
