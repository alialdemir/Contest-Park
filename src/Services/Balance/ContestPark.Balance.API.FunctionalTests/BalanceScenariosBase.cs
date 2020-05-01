using ContestPark.Balance.API.Enums;
using ContestPark.Core.Database.Models;
using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace ContestPark.Balance.API.FunctionalTests
{
    [Collection("Database remove")]
    public class BalanceScenariosBase : ScenariosBase<BalanceTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            //host.MigrateDatabase((services, updateDatabase) =>
            //{
            //    var settings = services.GetService<IOptions<BalanceSettings>>();

            //    var logger = services.GetService<ILogger<BalanceApiSeed>>();

            //    updateDatabase(
            //        settings.Value.ConnectionString,
            //        MigrationAssembly.GetAssemblies(),
            //        () =>
            //        {
            //            new BalanceApiSeed()
            //             .SeedAsync(services, logger)
            //             .Wait();
            //        });
            //});
        }

        public static class Entpoints
        {
            public static string GetBalance()
            {
                return "api/v1/balance";
            }

            public static string PostBalance()
            {
                return "api/v1/balance";
            }

            public static string GetBalanceByUserId(string userId, BalanceTypes? balanceType)
            {
                string url = $"api/v1/Balance/{userId}";

                if (balanceType != null)
                {
                    url += $"?balanceType={balanceType}";
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
