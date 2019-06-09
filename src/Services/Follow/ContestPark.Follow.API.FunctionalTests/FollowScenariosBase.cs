using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.IntegrationTests;
using ContestPark.Follow.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class FollowScenariosBase : ScenariosBase<FollowTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host
           .MigrateDatabase<FollowApiSeed>((services, logger) =>
           {
               new FollowApiSeed()
                   .SeedAsync(services, logger)
                   .Wait();
           });
        }

        public static class Entpoints
        {
            public static string Post(string followedUserId)
            {
                return "api/v1/follow/" + followedUserId;
            }

            public static string Delete(string followedUserId)
            {
                return "api/v1/follow/" + followedUserId;
            }

            public static string GetFollowing(string userId, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/follow/{userId}/Following" + Paginated(pageSize, pageNumber)
                    : $"api/v1/follow/{userId}/Following";
            }

            public static string GetFollowers(string userId, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/follow/{userId}/Followers" + Paginated(pageSize, pageNumber)
                    : $"api/v1/follow/{userId}/Followers";
            }

            private static string Paginated(int pageSize, int pageNumber)
            {
                return new PagingModel
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                }.ToString();
            }
        }
    }
}