using ContestPark.Core.Database.Models;
using ContestPark.Follow.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Infrastructure.MySql.Repositories
{
    public interface IFollowRepository
    {
        /// <param name="followUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        Task<bool> FollowAsync(string followUpUserId, string followedUserId);

        /// <param name="followUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        Task<bool> UnFollowAsync(string followUpUserId, string followedUserId);

        /// <param name="followUpUserId">Takip eden</param>
        /// <param name="followedUserId">Takip edilen</param>
        bool CheckFollowUpStatus(string FollowUpUserId, string followedUserId);

        ServiceModel<FollowUserModel> Followers(string userId, PagingModel pagingModel);

        ServiceModel<FollowUserModel> Following(string userId, PagingModel pagingModel);

        IEnumerable<string> GetFollowingUserIds(string userId, PagingModel paging);
    }
}
