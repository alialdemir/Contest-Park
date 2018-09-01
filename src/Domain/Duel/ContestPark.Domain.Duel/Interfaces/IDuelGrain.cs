using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Duel.Model.Request;
using System.Threading.Tasks;

namespace ContestPark.Domain.Duel.Interfaces
{
    public interface IDuelGrain : IGrainBase
    {
        Task DuelStart(DuelStart duelStart);

        Task UpdateTotalScores(int duelId, byte founderScore, byte opponentScore);
    }
}