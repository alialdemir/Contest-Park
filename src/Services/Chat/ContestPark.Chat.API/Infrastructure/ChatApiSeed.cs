using ContestPark.Chat.API.Infrastructure.Documents;
using ContestPark.Core.CosmosDb.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
                var conversation = new Conversation
                {
                    SenderUserId = "1111-1111-1111-1111",
                    LastMessage = "test",
                    LastWriterUserId = "1111-1111-1111-1111",
                    ReceiverUserId = "2222-2222-2222-2222"
                };

                await InsertDataAsync(new List<Conversation>
                {
                     conversation
                });

                await InsertDataAsync(new List<Message>
                {
                    new Message
                    {
                         AuthorUserId ="1111-1111-1111-1111",
                         ConversationId=conversation.Id,
                         Text = "test"
                    }
                });

                await InsertDataAsync(new List<Block>
                {
                    new Block
                    {
                         SkirterUserId = "1111-1111-1111-1111",
                         DeterredUserId = "3333-3333-3333-bot"
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