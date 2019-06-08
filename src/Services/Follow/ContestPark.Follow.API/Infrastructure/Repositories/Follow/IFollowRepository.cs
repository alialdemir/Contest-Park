﻿using System.Collections.Generic;
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
        bool IsFollowUpStatus(string followingUserId, string followedUserId);

        string[] Followers(string userId);

        string[] Following(string userId);
    }
}