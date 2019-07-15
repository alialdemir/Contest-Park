using ContestPark.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Core.Services.Identity
{
    public interface IIdentityService
    {
        Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds);

        Task<UserIdModel> GetUserIdByUserName(string userName);
    }
}
