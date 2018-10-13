using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Blocking
{
    public interface IBlockingService
    {
        Task Block(string userId);

        Task UnBlock(string userId);

        Task<ServiceModel<UserBlocking>> UserBlockingList(PagingModel pagingModel);
    }
}