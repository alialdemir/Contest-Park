using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Domain.Signalr.Model.Request;
using System.Threading.Tasks;

namespace ContestPark.Domain.Signalr.Interfaces
{
    public interface IContestParkHub
    {
        Task GetConnectionId(string connectionId);

        Task RemoveConnectionId(string connectionId);

        Task NextQuestion(NextQuestion questionCreated);

        Task DuelScreen(DuelStartingScreen duelStartingScreen);
    }
}