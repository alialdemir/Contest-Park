using ContestPark.Duel.API.Models;
using System.Collections.Generic;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer
{
    public interface IUserAnswerRepository
    {
        List<UserAnswerModel> GetAnswers(int deuelId);

        void Add(UserAnswerModel userAnswer);

        void AddRangeAsync(List<UserAnswerModel> userAnswers);
        void Remove(int duelId);
    }
}
