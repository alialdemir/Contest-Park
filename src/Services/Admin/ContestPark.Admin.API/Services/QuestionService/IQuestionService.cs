using ContestPark.Admin.API.Model.Question;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Services.QuestionService
{
    public interface IQuestionService
    {
        Task<List<QuestionSaveModel>> GenerateQuestion(QuestionConfigModel configModel);
    }
}
