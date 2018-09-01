using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Cp.Enums;
using System.Threading.Tasks;

namespace ContestPark.Domain.Cp.Interfaces
{
    public interface ICpGrain : IGrainBase
    {
        Task<int> GetTotalGoldByUserId(string userId);

        Task<int> RemoveGold(string userId, int diminishingGold, GoldProcessNames goldProcessName);

        Task<int> AddGold(string userId, int diminishingGold, GoldProcessNames goldProcessName);
    }
}