using ContestPark.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Core.Services.Identity
{
    public interface IIdentityService
    {
        Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds, bool includeCoverPicturePath = false);

        Task<string> GetRandomUserId();

        Task<UserNameModel> GetUserNameByPhoneNumber(string phoneNumber);
    }
}
