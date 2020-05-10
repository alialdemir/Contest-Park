using ContestPark.Core.Database.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Mission.API.Infrastructure
{
    public class MissionApiSeed : ContextSeedBase<MissionApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<MissionApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
            });
        }
    }
}
