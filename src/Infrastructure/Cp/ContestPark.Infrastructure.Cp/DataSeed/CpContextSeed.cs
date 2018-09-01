using ContestPark.Core.DataSeed;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Cp.Enums;
using ContestPark.Infrastructure.Cp.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Cp.DataSeed
{
    public class CpContextSeed : ContextSeedBase
    {
        public override async Task SeedAsync(ISettingsBase settings, ILogger logger)
        {
            var policy = CreatePolicy();

            await policy.ExecuteAsync(async () =>
            {
                ConnectionString = settings.ConnectionString;

                Logger = logger;
                SeedName = nameof(CpContextSeed);

                string userId = "1111-1111-1111-1111";
                string demoUserId = "2222-2222-2222-2222";
                string botUser = "3333-3333-3333-bot";

                await InsertDataAsync(GetCps(userId, demoUserId, botUser));

                await InsertDataAsync(GetCpInfo(userId, demoUserId, botUser));
            });
        }

        private IEnumerable<CpInfoEntity> GetCpInfo(string userId, string demoUserId, string botUser)
        {
            return new List<CpInfoEntity>
            {
                new CpInfoEntity
                {
                    UserId=userId,
                    CpSpent=999999999,
                    ChipProcessName= GoldProcessNames.DailyChip,
                },
                new CpInfoEntity
                {
                    UserId=demoUserId,
                    CpSpent=999999999,
                    ChipProcessName= GoldProcessNames.DailyChip,
                },
                new CpInfoEntity
                {
                    UserId=botUser,
                    CpSpent=999999999,
                    ChipProcessName= GoldProcessNames.DailyChip,
                }
            };
        }

        private IEnumerable<CpEntity> GetCps(string userId, string demoUserId, string botUser)
        {
            return new List<CpEntity>
            {
                new CpEntity
                {
                    CpAmount=999999999,
                    UserId = userId
                },
                new CpEntity
                {
                    CpAmount=999999999,
                    UserId=demoUserId
                },
                new CpEntity
                {
                    CpAmount=999999999,
                    UserId=botUser
                }
            };
        }
    }
}