using ContestPark.Core.Enums;
using ContestPark.Duel.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Question
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<QuestionModel>> DuelQuestions(short subCategoryId, string founderUserId, string opponentUserId, Languages founderLanguge, Languages opponentLanguge);

        Task<int> Insert(Tables.Question question);
    }
}
