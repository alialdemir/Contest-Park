namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer
{
    public interface IUserAnswerRepository
    {
        System.Collections.Generic.List<Models.UserAnswerModel> GetAnswers(Models.UserAnswerModel duelUser);
        void Insert(Models.UserAnswerModel userAnswer);
    }
}
