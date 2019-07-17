using ContestPark.Core.Dapper.Extensions;
using ContestPark.Core.Database.Extensions;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure;
using ContestPark.Duel.API.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace ContestPark.Duel.API.FunctionalTests
{
    [Collection("Database remove")]
    public class DuelScenariosBase : ScenariosBase<DuelTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host.MigrateDatabase((services, updateDatabase) =>
            {
                var settings = services.GetService<IOptions<DuelSettings>>();

                if (!settings.Value.IsMigrateDatabase)
                    return;

                var logger = services.GetService<ILogger<DuelApiSeed>>();

                updateDatabase(
                    settings.Value.ConnectionString,
                    MigrationAssembly.GetAssemblies(),
                    () =>
                    {
                        new DuelApiSeed()
                         .SeedAsync(services, logger)
                         .Wait();
                    });
            });
        }

        public static class Entpoints
        {
            public static string GetRankingBySubCategoryId(short subCategoryId, BalanceTypes balanceType, bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Ranking/SubCategory/{subCategoryId}" + Paginated(pageSize, pageNumber) + "&balanceType=" + balanceType.ToString()
                    : $"api/v1/Ranking/SubCategory/{subCategoryId}?balanceType=" + balanceType.ToString();
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
