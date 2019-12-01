using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.QuestionLocalized
{
    public interface IQuestionLocalizedRepository
    {
        Task<int> Insert(Tables.QuestionLocalized questionLocalized);

        int IsQuestionRegistry(string question);
    }
}
