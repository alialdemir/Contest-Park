using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public interface IFollowService
    {
        Task<bool> FollowUpAsync(string followedUserId);

        Task<bool> UnFollowAsync(string followedUserId);
    }
}