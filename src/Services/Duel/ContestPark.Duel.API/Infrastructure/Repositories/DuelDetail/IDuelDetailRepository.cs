using ContestPark.Duel.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail
{
    public interface IDuelDetailRepository
    {
        Task<bool> AddRangeAsync(IEnumerable<DuelDetailModel> duelDetails);

        Enums.Stylish GetCorrectAnswer(int duelId, int questionId);
        Task UpdateDuelDetailAsync(List<UserAnswerModel> userAnswers);
    }
}
