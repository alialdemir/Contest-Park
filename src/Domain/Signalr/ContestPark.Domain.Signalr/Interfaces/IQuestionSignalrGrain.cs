using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Domain.Signalr.Model.Request;
using System.Threading.Tasks;

namespace ContestPark.Domain.Signalr.Interfaces
{
    public interface IQuestionSignalrGrain : IGrainBase
    {
        Task NextQuestionAsync(NextQuestion nextQuestion);

        Task DuelStartingScreenAsync(DuelStartingScreen duelStartingScreen);

        Task RemoveGroup(int duelId, params string[] connectionIds);

        Task AddToGroup(int duelId, string connectionId);
    }
}