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
                await InsertDataAsync(new List<Documents.Balance>
                {
                    new Documents.Balance
                    {
                         UserId = "1111-1111-1111-1111",
                         BalanceAmounts = new List<Documents.BalanceAmount>
                         {
                             new Documents.BalanceAmount
                             {
                                  Amount = 99999999,
                                  BalanceType = Enums.BalanceTypes.Gold
                             },
                             new Documents.BalanceAmount
                             {
                                  Amount = 99999999,
                                  BalanceType = Enums.BalanceTypes.Money
                             },
                         }
                    },
                     new Documents.Balance
                    {
                         UserId = "2222-2222-2222-2222",
                         BalanceAmounts = new List<Documents.BalanceAmount>
                         {
                             new Documents.BalanceAmount
                             {
                                  Amount = 10000,
                                  BalanceType = Enums.BalanceTypes.Gold
                             },
                             new Documents.BalanceAmount
                             {
                                  Amount = 5432,
                                  BalanceType = Enums.BalanceTypes.Money
                             },
                         }
                    },
                     // Diğer kullanıcı bilerek eklenmedi
                });
            });
        }
    }
}
