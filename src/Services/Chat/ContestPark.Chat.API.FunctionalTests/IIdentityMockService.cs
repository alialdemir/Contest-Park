using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.FunctionalTests
{
    public class IIdentityMockService : IIdentityService
    {
        public Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds)
        {
            return Task.FromResult(new List<UserModel>
            {
                new UserModel
                {
                    UserId = "1111-1111-1111-1111",
                    ProfilePicturePath = "http://i.pravatar.cc/150?u=witcherfearless",
                    FullName = "Ali Aldemir",
                    UserName = "witcherfearless",
                },
                new UserModel
                {
                    UserId = "2222-2222-2222-2222",
                    ProfilePicturePath = "http://i.pravatar.cc/150?u=demo",
                    FullName = "Demo",
                    UserName = "demo",
                },
                new UserModel
                {
                    UserId = "3333-3333-3333-bot",
                    ProfilePicturePath = "http://i.pravatar.cc/150?u=bot",
                    FullName = "Bot",
                    UserName = "bot12345",
                }
            }.AsEnumerable());
        }
    }
}
