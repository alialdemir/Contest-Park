using ContestPark.Core.CosmosDb.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Follow.API.Infrastructure
{
    public class FollowApiSeed : ContextSeedBase<FollowApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<FollowApiSeed> logger)
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