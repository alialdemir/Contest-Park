using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Extensions;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using ContestPark.Post.API.Infrastructure;
using ContestPark.Post.API.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace ContestPark.Post.API.FunctionalTests
{
    [Collection("Database remove")]
    public class PostScenariosBase : ScenariosBase<PostTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host.MigrateDatabase((services, updateDatabase) =>
            {
                var settings = services.GetService<IOptions<PostSettings>>();

                if (!settings.Value.IsMigrateDatabase)
                    return;

                var logger = services.GetService<ILogger<PostApiSeed>>();

                updateDatabase(
                    settings.Value.ConnectionString,
                    MigrationAssembly.GetAssemblies(),
                    () =>
                    {
                        new PostApiSeed()
                         .SeedAsync(services, logger)
                         .Wait();
                    });
            });
        }

        public static class Entpoints
        {
            public static string PostLike(int postId)
            {
                return $"api/v1/Post/{postId}/Like";
            }

            public static string PostComment(int postId)
            {
                return $"api/v1/Post/{postId}/Comment";
            }

            public static string DeleteUnLike(int postId)
            {
                return $"api/v1/Post/{postId}/UnLike";
            }

            public static string GetPostLikes(int postId, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Post/{postId}/Like" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Post/{postId}/Like";
            }

            public static string GetPostsBySubcategoryId(int subcategoryId, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Post/subcategory/{subcategoryId}" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Post/subcategory/{subcategoryId}";
            }

            public static string GetPostsByUserIdAsync(string userName, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Post/subcategory/User/{userName}" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Post/subcategory/User/{userName}";
            }

            public static string GetPostDetail(int postId, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Post/{postId}" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Post/{postId}";
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
