using ContestPark.Duel.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public interface IDuelRepository
    {
        Models.DuelResultModel DuelResultByDuelId(int duelId, string userId);

        Tables.Duel GetDuelByDuelId(int duelId);

        DuelBalanceInfoModel GetDuelBalanceInfoByDuelId(int duelId);

        Task<int?> Insert(Tables.Duel duel);

        bool IsDuelFinish(int duelId);

        Task<bool> UpdateAsync(Tables.Duel duel);
    }
}
