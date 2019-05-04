using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public class FollowMockService : IFollowService
    {
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