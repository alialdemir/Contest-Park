using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.FunctionalTests;
using ContestPark.Post.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace ContestPark.Post.API.FunctionalTests
{
    [Collection("Database remove")]
    public class PostScenariosBase : ScenariosBase<PostTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host
           .MigrateDatabase<PostApiSeed>((services, logger) =>
           {
               new PostApiSeed()
                   .SeedAsync(services, logger)
                   .Wait();
           });
        }

        public static class Entpoints
        {
            public static string PostLike(string postId)
            {
                return $"api/v1/Post/{postId}/Like";
            }

            public static string DeleteUnLike(string postId)
            {
                return $"api/v1/Post/{postId}/UnLike";
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
