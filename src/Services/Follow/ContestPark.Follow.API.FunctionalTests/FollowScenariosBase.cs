using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Extensions;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using ContestPark.Follow.API.Infrastructure;
using ContestPark.Follow.API.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace ContestPark.Follow.API.FunctionalTests
{
    [Collection("Database remove")]
    public class FollowScenariosBase : ScenariosBase<FollowTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host.MigrateDatabase((services, updateDatabase) =>
            {
                var settings = services.GetService<IOptions<FollowSettings>>();

                if (!settings.Value.IsMigrateDatabase)
                    return;

                var logger = services.GetService<ILogger<FollowApiSeed>>();

                updateDatabase(
                    settings.Value.ConnectionString,
                    MigrationAssembly.GetAssemblies(),
                    () =>
                    {
                        new FollowApiSeed()
                            .SeedAsync(services, logger)
                            .Wait();
                    });
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
