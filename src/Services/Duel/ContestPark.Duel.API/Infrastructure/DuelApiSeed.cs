using ContestPark.Core.Database.Infrastructure;
using ContestPark.Duel.API.Infrastructure.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure
{
    public class DuelApiSeed : ContextSeedBase<DuelApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<DuelApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                await InsertDataAsync(new List<ContestDate>
                {
                    new ContestDate
                    {
                        StartDate = DateTime.Now,
                        FinishDate = DateTime.Now.AddDays(30)
                    }
                });

                await InsertDataAsync(new List<ScoreRanking>
                {
                    new ScoreRanking
                    {
                        UserId = "1111-1111-1111-1111",
                        SubCategoryId = 1,
                        DisplayTotalGoldScore = "10k",
                        DisplayTotalMoneyScore = "20k",
                        TotalGoldScore = 10000,
                        TotalMoneyScore = 20000,
                        ContestDateId = 1,
                    },
                    new ScoreRanking
                    {
                        UserId = "2222-2222-2222-2222",
                        SubCategoryId = 1,
                        DisplayTotalGoldScore = "5k",
                        DisplayTotalMoneyScore = "5",
                        TotalGoldScore = 5000,
                        TotalMoneyScore = 5,
                        ContestDateId = 1,
                    },
                    new ScoreRanking
                    {
                        UserId = "3333-3333-3333-bot",
                        SubCategoryId = 1,
                        DisplayTotalGoldScore = "134",
                        DisplayTotalMoneyScore = "1",
                        TotalGoldScore = 134,
                        TotalMoneyScore = 1,
                        ContestDateId = 1,
                    },
                });

                await InsertDataAsync(new List<BalanceRanking>
                {
                    new BalanceRanking
                    {
                        UserId = "1111-1111-1111-1111",
                        ContestDateId = 1,
                        DisplayTotalGold = "10k",
                        TotalGold = 10000,
                        DisplayTotalMoney = "20 TL",
                        TotalMoney = 20.00M,
                    },
                    new BalanceRanking
                    {
                        UserId = "2222-2222-2222-2222",
                        ContestDateId = 1,
                        DisplayTotalGold = "5k",
                        TotalGold = 5000,
                        DisplayTotalMoney = "5 TL",
                        TotalMoney = 5.00M,
                    },
                    new BalanceRanking
                    {
                        UserId = "3333-3333-3333-bot",
                        ContestDateId = 1,
                        DisplayTotalGold = "134",
                        TotalGold = 134,
                        DisplayTotalMoney = "1 TL",
                        TotalMoney = 1.00M,
                    },
                });
            });
        }
    }
}
