using ContestPark.Chat.API.Model;
using ContestPark.Core.CosmosDb.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Block
{
    public interface IBlockRepository
    {
        Task<bool> BlockedAsync(string skirterUserId, string deterredUserId);

        bool BlockingStatus(string skirterUserId, string deterredUserId);

        Task<bool> UnBlockAsync(string skirterUserId, string deterredUserId);

        ServiceModel<BlockModel> UserBlockedList(string SkirterUserId, PagingModel paging);
    }
}