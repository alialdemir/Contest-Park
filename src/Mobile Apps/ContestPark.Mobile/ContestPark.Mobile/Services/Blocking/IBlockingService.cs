using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Blocking
{
    public interface IBlockingService
    {
        Task<bool> Block(string userId);

        Task<ServiceModel<BlockModel>> BlockingList(PagingModel pagingModel);

        Task<bool> BlockingStatusAsync(string senderUserId);

        Task<bool> UnBlock(string userId);
    }
}