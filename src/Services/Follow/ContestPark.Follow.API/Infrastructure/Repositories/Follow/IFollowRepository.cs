using ContestPark.Core.CosmosDb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Infrastructure.Repositories.Follow
{
    public interface IFollowRepository
    {
        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        Task<bool> FollowAsync(string followingUserId, string followedUserId);

        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        Task<bool> UnFollowAsync(string followingUserId, string followedUserId);

        /// <param name="followingUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        bool CheckFollowUpStatus(string followingUserId, string followedUserId);

        IEnumerable<string> CheckFollowUpStatus(string followingUserId, IEnumerable<string> userIds);

        IEnumerable<string> Followers(string userId, PagingModel pagingModel);

        IEnumerable<string> Following(string userId, PagingModel pagingModel);
    }
}