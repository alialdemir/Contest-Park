using ContestPark.Core.Database.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                await InsertDataAsync(new List<Tables.Follow>
                {
                        new Tables.Follow
                    {
                         FollowUpUserId = "2222-2222-2222-2222",
                         FollowedUserId = "1111-1111-1111-1111",
                    },
                    new Tables.Follow
                    {
                         FollowUpUserId = "1111-1111-1111-1111",
                         FollowedUserId = "2222-2222-2222-2222"
                    },
                    new Tables.Follow
                    {
                         FollowUpUserId = "2222-2222-2222-2222",
                         FollowedUserId = "3333-3333-3333-bot"
                    },
                    new Tables.Follow
                    {
                         FollowUpUserId = "3333-3333-3333-bot",
                         FollowedUserId = "2222-2222-2222-2222"
                    }
                });
            });
        }
    }
}
