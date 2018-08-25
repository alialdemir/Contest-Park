using ContestPark.Domain.Duel.Model.Request;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Duel.Interfaces
{
    public interface IDuelGrain : IGrainWithIntegerKey
    {
        Task DuelStart(DuelStart duelStart);

        Task SaveUserAnswer(UserAnswer userAnswer);

        Task SaveUserAnswerProcess(UserAnswer userAnswer);

        Task<bool> IsGameEnd(int duelId);
    }
}