using ContestPark.Domain.Question.Model.Request;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Repositories.Question
{
    public interface IQuestionRepository
    {
        Task<Domain.Question.Model.Response.Question> GetQuestionAsync(QuestionInfo questionInfo);
    }
}