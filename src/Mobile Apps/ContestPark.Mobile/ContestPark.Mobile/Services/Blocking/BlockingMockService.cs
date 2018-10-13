using ContestPark.Mobile.Models.Blocking;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Blocking
{
    internal class BlockingMockService : IBlockingService
    {
        public Task Block(string userId)
        {
            return Task.CompletedTask;
        }

        public Task UnBlock(string userId)
        {
            return Task.CompletedTask;
        }

        public Task<ServiceModel<UserBlocking>> UserBlockingList(PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<UserBlocking>
            {
                Items = new List<UserBlocking>
                 {
                     new UserBlocking
                     {
                         FullName ="Ali Aldemir",
                         UserName = "witcherfearless",
                         UserId ="1111-1111-1111-1111"
                     },
                     new UserBlocking
                     {
                         FullName ="demo",
                         UserName = "demo",
                         UserId = "2222-2222-2222-2222"
                     }
                 }
            });
        }
    }
}