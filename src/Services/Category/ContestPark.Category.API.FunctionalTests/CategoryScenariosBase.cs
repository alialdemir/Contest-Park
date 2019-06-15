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
                ISearchRepository searchRepository = (ISearchRepository)services.GetRequiredService(typeof(ISearchRepository));
                searchRepository.CreateSearchIndexs();

                new CategoryApiSeed()
                    .SeedAsync(services, logger)
                    .Wait();
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

            public static string PostFollowSubCategories(string subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/Follow";
            }

            public static string DeleteUnFollowSubCategories(string subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/UnFollow";
            }

            public static string GetFollowStatus(string subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/FollowStatus";
            }

            public static string GetSubcategoryDetail(string subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}";
            }

            public static string PostUnLockSubcategory(string subCategoryId)
            {
                return $"api/v1/subcategory/{subCategoryId}/unlock";
            }

            public static string GetSearch(string categoryId, string searchText, bool paginated = false, int pageSize = 10, int pageNumber = 1)
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
                else if (!string.IsNullOrEmpty(categoryId))
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