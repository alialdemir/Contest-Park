using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public class FollowMockService : IFollowService
    {
        public Task<ServiceModel<FollowModel>> Followers(string followedUserId, PagingModel pagingModel)
        {
            List<FollowModel> follows = new List<FollowModel>();

            for (byte i = 0; i < 10; i++)
            {
                follows.Add(new FollowModel
                {
                    IsFollowing = i % 2 == 0,
                    FullName = "User" + i.ToString(),
                    ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    UserId = "1111-1111-1111-111" + i.ToString(),
                    UserName = "witcher" + i.ToString(),
                });
            }

            return Task.FromResult(new ServiceModel<FollowModel>
            {
                Count = 2,
                PageNumber = 1,
                PageSize = 1,
                Items = follows
            });
        }

        public Task<bool> FollowUpAsync(string followedUserId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UnFollowAsync(string followedUserId)
        {
            return Task.FromResult(true);
        }
    }
}