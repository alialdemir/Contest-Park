using ContestPark.Domain.Question.Model.Response;
using ContestPark.Domain.Signalr.Model.Request;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Signalr.Interfaces
{
    public interface IQuestionSignalrGrain : IGrainWithIntegerKey
    {
        Task NextQuestionAsync(QuestionCreated questionCreated);

        Task DuelStartingScreenAsync(DuelStartingScreen duelStartingScreen);
    }
}