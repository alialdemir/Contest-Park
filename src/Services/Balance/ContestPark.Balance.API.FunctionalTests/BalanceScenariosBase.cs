using ContestPark.Balance.API.Enums;
using ContestPark.Balance.API.Infrastructure;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
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
            host.MigrateDatabase<BalanceApiSeed>((services, logger) =>
            {
                new BalanceApiSeed()
                    .SeedAsync(services, logger)
                    .Wait();
            });
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