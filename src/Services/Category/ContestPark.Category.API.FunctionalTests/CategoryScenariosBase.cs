using ContestPark.Category.API.Infrastructure;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Category.API.FunctionalTests
{
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