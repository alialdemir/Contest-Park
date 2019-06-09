using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Follow.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Xunit;

namespace ContestPark.Follow.API.FunctionalTests
{
    [Collection("Database remove")]
    [TestCaseOrderer("ContestPark.Follow.API.FunctionalTests.TestCaseOrdering.PriorityOrderer", "ContestPark.Follow.API.FunctionalTests")]
    public class FollowScenariosBase

    {
        public static IConfiguration Configuration { get; set; }

        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(FollowScenariosBase))
              .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                  .ConfigureAppConfiguration(cb =>
                  {
                      cb.AddJsonFile("appsettings.test.json", optional: false);
                      cb.AddEnvironmentVariables();
                  })
                .UseStartup<FollowTestStartup>();

            var testServer = new TestServer(hostBuilder);

            testServer.Host
             .MigrateDatabase<FollowApiSeed>((services, logger) =>
             {
                 new FollowApiSeed()
                     .SeedAsync(services, logger)
                     .Wait();
             });

            FollowScenariosBase.Configuration = testServer.Host.Services.GetRequiredService<IConfiguration>();

            return testServer;
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