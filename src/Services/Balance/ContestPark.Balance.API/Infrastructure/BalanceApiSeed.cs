using ContestPark.Balance.API.Infrastructure.Tables;
using ContestPark.Core.Database.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure
{
    public class BalanceApiSeed : ContextSeedBase<BalanceApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<BalanceApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                await InsertDataAsync<string, Tables.Balance>(new List<Tables.Balance>
                {
                    new Tables.Balance
                    {
                         UserId = "1111-1111-1111-1111",
                            Gold=100000,
                            Money = 500000,
                    },
                     new Tables.Balance
                    {
                         UserId = "2222-2222-2222-2222",
                         Gold =10000,
                         Money=5432
                    },
                });

                await InsertDataAsync(new List<BalanceHistory>
                {
                    new BalanceHistory
                    {
                        BalanceHistoryType = Enums.BalanceHistoryTypes.Boost,
                        BalanceType= Enums.BalanceTypes.Gold,
                        NewAmount=34,
                        OldAmount=77,
                        UserId = "1111-1111-1111-1111",
                    }
                });
            });
        }
    }
}
