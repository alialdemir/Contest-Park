using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Follow
{
    public interface IFollowService
    {
        Task<bool> FollowStatusAsync(string userId1, string userId2);
    }
}
