using ContestPark.Duel.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail
{
    public interface IDuelDetailRepository
    {
        Task<bool> AddRangeAsync(IEnumerable<Tables.DuelDetail> duelDetails);
    }
}
