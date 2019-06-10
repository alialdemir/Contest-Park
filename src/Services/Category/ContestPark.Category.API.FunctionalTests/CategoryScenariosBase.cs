using ContestPark.Category.API.Infrastructure;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ContestPark.Category.API.FunctionalTests
{
    [Collection("Database remove")]
    public class CategoryScenariosBase : ScenariosBase<CategoryTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host.MigrateDatabase<CategoryApiSeed>((services, logger) =>
            {
                new CategoryApiSeed()
                    .SeedAsync(services, logger)
                    .Wait();

                ISearchRepository searchRepository = (ISearchRepository)services.GetRequiredService(typeof(ISearchRepository));
                searchRepository.CreateCategoryIndex();
            });
        }

        public static class Entpoints
        {
            public static string Get(bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/subcategory" + Paginated(pageSize, pageNumber)
                    : $"api/v1/subcategory";
            }

            public static string GetFollowedSubCategories(bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Subcategory/Followed" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Subcategory/Followed";
            }

            public static string GetSearchFollowedSubCategories(string searchText, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Search/Followed" + Paginated(pageSize, pageNumber) + "&q=" + searchText
                    : $"api/v1/Search/Followed?q=" + searchText;
            }

            public static string Paginated(int pageSize, int pageNumber)
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