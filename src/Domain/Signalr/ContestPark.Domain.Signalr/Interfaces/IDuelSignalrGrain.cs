using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Signalr.Model.Request;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Signalr.Interfaces
{
    public interface IDuelSignalrGrain : IGrainBase
    {
        Task WaitingOpponentAsync(WaitingOpponent waitingOpponent);

        Task WaitingOpponentRemoveAsync(WaitingOpponentRemove opponentRemove);
    }
}