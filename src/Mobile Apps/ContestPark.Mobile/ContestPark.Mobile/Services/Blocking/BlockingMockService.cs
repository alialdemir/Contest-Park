using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Blocking
{
    internal class BlockingMockService : IBlockingService
    {
        private bool isBlocking = false;

        public async Task<bool> Block(string userId)
        {
            await Task.Delay(5000);
            isBlocking = true;
            return true;
        }

        public Task<ServiceModel<BlockModel>> BlockingList(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<BlockModel>
            {
                Items = new List<BlockModel>
                 {
                     new BlockModel
                     {
                         FullName ="Ali Aldemir",
                         UserName = "witcherfearless",
                         UserId ="1111-1111-1111-1111"
                     },
                     new BlockModel
                     {
                         FullName ="demo",
                         UserName = "demo",
                         UserId = "2222-2222-2222-2222"
                     }
                 }
            });
        }

        public Task<bool> BlockingStatusAsync(string senderUserId)
        {
            return Task.FromResult(isBlocking);
        }

        public async Task<bool> UnBlock(string userId)
        {
            await Task.Delay(5000);
            isBlocking = false;
            return true;
        }
    }
}