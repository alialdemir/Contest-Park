using ContestPark.Domain.Cp.Enums;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Cp.Interfaces
{
    public interface ICpGrain : IGrainWithIntegerKey
    {
        Task<int> GetTotalGoldByUserId(string userId);

        Task<int> RemoveGold(string userId, int diminishingGold, GoldProcessNames goldProcessName);
    }
}