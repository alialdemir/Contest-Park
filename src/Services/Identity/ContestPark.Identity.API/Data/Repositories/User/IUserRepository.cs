using ContestPark.Core.Models;
using ContestPark.Identity.API.Models;
using System.Collections.Generic;

namespace ContestPark.Identity.API.Data.Repositories.User
{
    public interface IUserRepository
    {
        bool CodeCheck(int code);

        ApplicationUser GetUserByCode(int code);

        int InsertCode(string userId);

        void RemoveCode(string userId);

        IEnumerable<UserModel> GetUserInfos(List<string> userInfos, bool includeCoverPicturePath = false);

        UserProfileModel GetUserInfoByUserName(string userName);

        IEnumerable<string> GetRandomProfilePictures();

        string GetRandomBotUserId();
    }
}
