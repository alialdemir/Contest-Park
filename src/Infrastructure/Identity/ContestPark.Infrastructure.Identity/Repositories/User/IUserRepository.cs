using ContestPark.Core.Domain.Model;

namespace ContestPark.Infrastructure.Identity.Repositories.User
{
    public interface IUserRepository
    {
        ServiceResponse<string> RandomUserProfilePictures(string userId, Paging pagingModel);
    }
}