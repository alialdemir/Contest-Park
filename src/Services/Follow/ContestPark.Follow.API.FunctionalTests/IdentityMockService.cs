using ContestPark.Core.Models;
using ContestPark.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class IdentityMockService : IIdentityService
    {
        private readonly List<UserModel> users;

        public IdentityMockService()
        {
            users = new List<UserModel>
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
            };
        }

        public Task<UserIdModel> GetUserIdByUserName(string userName)
        {
            var user = users.Where(u => u.UserName == userName).Select(u => new UserIdModel
            {
                UserId = u.UserId
            }).FirstOrDefault();

            return Task.FromResult(user);
        }

        public Task<string> GetRandomUserId()
        {
            return Task.FromResult(users.OrderBy(x => Guid.NewGuid()).FirstOrDefault().UserId);
        }

        public Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds, bool includeCoverPicturePath = false)
        {
            return Task.FromResult(users.AsEnumerable());
        }
    }
}
