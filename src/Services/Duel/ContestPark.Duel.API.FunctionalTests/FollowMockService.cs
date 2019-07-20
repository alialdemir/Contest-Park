using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Services.Follow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.FunctionalTests
{
    public class FollowMockService : IFollowService
    {
        public Task<IEnumerable<string>> GetFollowingUserIds(string userId, PagingModel pagingModel)
        {
            return Task.FromResult(new List<string>
            {
                "2222-2222-2222-2222"
            }.AsEnumerable());
        }
    }
}
