using ContestPark.Domain.Question.Model.Request;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Question.Interfaces
{
    public interface IQuestionGrain : IGrainWithIntegerKey
    {
        Task QuestionCreate(QuestionInfo questionInfo);
    }
}