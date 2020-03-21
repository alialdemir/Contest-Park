using ContestPark.Admin.API.Model.Question;
using System.Collections.Generic;

namespace ContestPark.Admin.API.Services.QuestionService
{
    public interface IQuestionService
    {
        List<QuestionSaveModel> GenerateQuestion(QuestionConfigModel configModel);
    }
}
