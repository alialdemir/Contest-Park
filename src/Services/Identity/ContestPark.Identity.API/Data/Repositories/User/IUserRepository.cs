using ContestPark.Identity.API.Models;

namespace ContestPark.Identity.API.Data.Repositories.User
{
    public interface IUserRepository
    {
        bool CodeCheck(int code);

        ApplicationUser GetUserByCode(int code);

        int InsertCode(string userId);

        void RemoveCode(string userId);
    }
}