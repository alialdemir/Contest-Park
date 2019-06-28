using ContestPark.Core.CosmosDb.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Post.API.Infrastructure
{
    public class PostApiSeed : ContextSeedBase<PostApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<PostApiSeed> logger)
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