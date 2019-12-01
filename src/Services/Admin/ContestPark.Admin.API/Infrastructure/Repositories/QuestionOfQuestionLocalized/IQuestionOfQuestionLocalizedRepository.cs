using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.QuestionOfQuestionLocalized
{
    public interface IQuestionOfQuestionLocalizedRepository
    {
        Task<bool> Insert(Tables.QuestionOfQuestionLocalized questionOfQuestion);
    }
}
