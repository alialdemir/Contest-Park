using ContestPark.Chat.API.Infrastructure.Tables;
using ContestPark.Core.Database.Infrastructure;
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
                var conversation1 = new Conversation
                {
                    SenderUserId = "1111-1111-1111-1111",
                    LastMessage = "test",
                    LastWriterUserId = "1111-1111-1111-1111",
                    ReceiverUserId = "2222-2222-2222-2222",
                    SenderUnreadMessageCount = 2
                };

                var conversation2 = new Conversation
                {
                    SenderUserId = "3333-3333-3333-bot",
                    LastMessage = "test",
                    LastWriterUserId = "3333-3333-3333-bot",
                    ReceiverUserId = "2222-2222-2222-2222"
                };

                await InsertDataAsync(new List<Conversation>
                {
                     conversation1,
                     conversation2
                });

                await InsertDataAsync(new List<Message>
                {
                    new Message
                    {
                         AuthorUserId ="1111-1111-1111-1111",
                         ConversationId = 1,
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
            });
        }
    }
}
