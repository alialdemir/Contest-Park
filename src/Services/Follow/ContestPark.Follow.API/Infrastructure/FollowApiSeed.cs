using ContestPark.Core.CosmosDb.Infrastructure;
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
                await InsertDataAsync(new List<Documents.Follow>
                {
                    new Documents.Follow
                    {
                         Id = "1111-1111-1111-1111",
                         Followers = new   List<string>{ "2222-2222-2222-2222",  },
                         Following =  new   List<string>{ "2222-2222-2222-2222", "3333-3333-3333-bot" }
                    },
                    new Documents.Follow
                    {
                         Id = "2222-2222-2222-2222",
                         Followers = new   List<string>{ "1111-1111-1111-1111",  },
                         Following =  new  List<string>{ "1111-1111-1111-1111", "3333-3333-3333-bot" }
                    },
                    new Documents.Follow
                    {
                         Id = "3333-3333-3333-bot",
                         Followers = new    List<string>{ "1111-1111-1111-1111", "2222-2222-2222-2222"  },
                    }
                });
            });
        }
    }
}