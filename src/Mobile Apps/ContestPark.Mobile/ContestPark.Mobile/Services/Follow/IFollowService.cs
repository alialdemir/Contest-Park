using ContestPark.Mobile.Models.Follow;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Follow
{
    public interface IFollowService
    {
        Task<ServiceModel<FollowModel>> Followers(string followedUserId, PagingModel pagingModel);

        Task<ServiceModel<FollowModel>> Following(string followedUserId, PagingModel pagingModel);

        Task<bool> FollowUpAsync(string followedUserId);

        Task<bool> UnFollowAsync(string followedUserId);
    }
}