using ContestPark.Core.Database.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
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
            });
        }
    }
}
