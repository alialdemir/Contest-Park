using ContestPark.Core.Domain.Interfaces;
using ContestPark.Domain.Cp.Enums;
using System.Threading.Tasks;

namespace ContestPark.Domain.Cp.Interfaces
{
    public interface ICpGrain : IGrainBase
    {
        Task<int> AddGold(string userId, int diminishingGold, GoldProcessNames goldProcessName);

        Task<int> GetTotalGoldByUserId(string userId);

        Task<int> RemoveGold(string userId, int diminishingGold, GoldProcessNames goldProcessName);
    }
}