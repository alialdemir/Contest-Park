using ContestPark.Core.CosmosDb.Infrastructure;
using ContestPark.Follow.API.Infrastructure.Documents;
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
                         FollowUpUserId = "2222-2222-2222-2222",
                         FollowedUserId = "1111-1111-1111-1111",
                    },
                    new Documents.Follow
                    {
                         FollowUpUserId = "1111-1111-1111-1111",
                         FollowedUserId = "2222-2222-2222-2222"
                    },
                    new Documents.Follow
                    {
                         FollowUpUserId = "2222-2222-2222-2222",
                         FollowedUserId = "3333-3333-3333-bot"
                    },
                    new Documents.Follow
                    {
                         FollowUpUserId = "3333-3333-3333-bot",
                         FollowedUserId = "2222-2222-2222-2222"
                    }
                });

                await InsertDataAsync(new List<User>
                {
                    new User
                    {
                            Id = "1111-1111-1111-1111",
                            ProfilePicturePath = "http://i.pravatar.cc/150?u=witcherfearless",
                            FullName = "Ali Aldemir",
                            UserName = "witcherfearless",
                    },
                    new User
                    {
                        Id = "2222-2222-2222-2222",
                        ProfilePicturePath = "http://i.pravatar.cc/150?u=demo",
                        FullName = "Demo",
                        UserName = "demo",
                    }, new User
                    {
                        Id = "3333-3333-3333-bot",
                        ProfilePicturePath = "http://i.pravatar.cc/150?u=bot",
                        FullName = "Bot",
                        UserName = "bot12345",
                    }
                });
            });
        }
    }
}