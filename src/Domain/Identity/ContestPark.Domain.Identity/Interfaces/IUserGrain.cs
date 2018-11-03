using ContestPark.Core.Domain.Interfaces;
using ContestPark.Core.Domain.Model;
using System.Threading.Tasks;

namespace ContestPark.Domain.Identity.Interfaces
{
    public interface IUserGrain : IGrainBase
    {
        Task<string> GetRandomBotUserId();

        Task<ServiceResponse<string>> RandomUserProfilePictures(string userId, Paging pagingModel);
    }
}