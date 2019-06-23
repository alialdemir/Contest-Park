using ContestPark.Core.CosmosDb.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure
{
    public class ChatApiSeed : ContextSeedBase<ChatApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<ChatApiSeed> logger)
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