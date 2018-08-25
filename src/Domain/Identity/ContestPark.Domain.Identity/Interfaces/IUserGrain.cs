using ContestPark.Core.Domain.Model;
using Orleans;
using System.Threading.Tasks;

namespace ContestPark.Domain.Identity.Interfaces
{
    public interface IUserGrain : IGrainWithIntegerKey
    {
        Task<ServiceResponse<string>> RandomUserProfilePictures(string userId, Paging pagingModel);
    }
}