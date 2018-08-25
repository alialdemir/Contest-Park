using ContestPark.Domain.Signalr.Model.Request;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Signalr.Interfaces
{
    public interface IDuelSignalrGrain : IGrainWithIntegerKey
    {
        Task WaitingOpponentAsync(WaitingOpponent waitingOpponent);

        Task WaitingOpponentRemoveAsync(WaitingOpponentRemove opponentRemove);
    }
}