using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.AnswerLocalized
{
    public interface IAnswerLocalizedRepository
    {
        Task<bool> Insert(List<Tables.AnswerLocalized> answerLocalizeds);
    }
}
