using ContestPark.Core.Dapper;
using ContestPark.Infrastructure.Duel.Entities;

namespace ContestPark.Infrastructure.Duel.Repositories.DuelInfo
{
    public interface IDuelInfoRepository : IRepository<DuelInfoEntity>
    {
        DuelInfoEntity GetDuelInfoByDuelId(int duelId, int questionInfoId);

        bool IsGameEnd(int duelId, byte gameCount = 7);
    }
}