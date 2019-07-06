using ContestPark.Category.API.Infrastructure;
using ContestPark.Category.API.Infrastructure.Repositories.Search;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Migrations;
using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Extensions;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace ContestPark.Category.API.FunctionalTests
{
    [Collection("Database remove")]
    public class CategoryScenariosBase : ScenariosBase<CategoryTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host.MigrateDatabase((services, updateDatabase) =>
            {
                var settings = services.GetService<IOptions<CategorySettings>>();

                if (!settings.Value.IsMigrateDatabase)
                    return;

                updateDatabase(
                    settings.Value.ConnectionString,
                    MigrationAssembly.GetAssemblies(),
                    () =>
                    {
                        var logger = services.GetService<ILogger<CategoryApiSeed>>();

                        ISearchRepository searchRepository = (ISearchRepository)services.GetRequiredService(typeof(ISearchRepository));
                        searchRepository.CreateSearchIndexs();

                        new CategoryApiSeed()
                           .SeedAsync(services, logger)
                           .Wait();
                    });
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
                    ? $"api/v1/Search/Followed{Paginated(pageSize, pageNumber)}&q={searchText}"
                    : $"api/v1/Search/Followed?q={searchText}";
            }

            public static string PostFollowSubCategories(short subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/Follow";
            }

            public static string DeleteUnFollowSubCategories(short subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/UnFollow";
            }

            public static string GetFollowStatus(short subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/FollowStatus";
            }

            public static string GetSubcategoryDetail(short subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}";
            }

            public static string PostUnLockSubcategory(short subCategoryId, BalanceTypes balanceType)
            {
                return $"api/v1/subcategory/{subCategoryId}/unlock?balanceType={balanceType}";
            }

            public static string GetSearch(short categoryId, string searchText, bool paginated = false, int pageSize = 10, int pageNumber = 1)
            {
                string url = "api/v1/Search";

                if (paginated)
                {
                    url += Paginated(pageSize, pageNumber);
                }

                if (!paginated && !string.IsNullOrEmpty(searchText))
                {
                    url += "?q=" + searchText;
                }
                else if (!string.IsNullOrEmpty(searchText))
                {
                    url += "&q=" + searchText;
                }

                if (paginated || !string.IsNullOrEmpty(searchText))
                {
                    url += "&categoryId=" + categoryId;
                }
                else if (categoryId != 0)
                {
                    url += "?categoryId=" + categoryId;
                }

                return url;
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
