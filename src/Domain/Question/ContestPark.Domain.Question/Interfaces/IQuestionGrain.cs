using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Question.Model.Request;
using System.Threading.Tasks;

namespace ContestPark.Domain.Question.Interfaces
{
    public interface IQuestionGrain : IGrainBase
    {
        Task<Domain.Question.Model.Response.Question> QuestionCreate(QuestionInfo questionInfo);
    }
}