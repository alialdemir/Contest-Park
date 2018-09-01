using ContestPark.Core.Dapper;
using ContestPark.Domain.Cp.Enums;
using ContestPark.Infrastructure.Cp.Entities;

namespace ContestPark.Infrastructure.Cp.Repositories.Cp
{
    public interface ICpRepository : IRepository<CpEntity>
    {
        int GetTotalGoldByUserId(string userId);

        int RemoveGold(string userId, int diminishingGold, GoldProcessNames goldProcessName);

        int AddGold(string userId, int addedChips, GoldProcessNames goldProcessName);
    }
}