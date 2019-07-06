using ContestPark.Core.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Infrastructure.Repositories.Follow
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
        bool CheckFollowUpStatus(string followUpUserId, string followedUserId);

        IEnumerable<string> CheckFollowUpStatus(string followUpUserId, IEnumerable<string> userIds);

        ServiceModel<string> Followers(string userId, PagingModel pagingModel);

        ServiceModel<string> Following(string userId, PagingModel pagingModel);
    }
}