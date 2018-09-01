using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Duel.Model.Request;
using System.Threading.Tasks;

namespace ContestPark.Domain.Duel.Interfaces
{
    public interface IGameGrain : IGrainBase
    {
        Task SaveUserAnswerProcess(UserAnswer userAnswer);

        Task SetState(GameState gameState);
    }
}