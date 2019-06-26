using ContestPark.Chat.API.Infrastructure;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace ContestPark.Chat.API.FunctionalTests
{
    [Collection("Database remove")]
    public class ChatScenariosBase : ScenariosBase<ChatTestStartup>
    {
        public override void Seed(IWebHost host)
        {
            host.MigrateDatabase<ChatApiSeed>((services, logger) =>
            {
                new ChatApiSeed()
                    .SeedAsync(services, logger)
                    .Wait();
            });
        }

        public static class Entpoints
        {
            public static string Post()
            {
                return $"api/v1/Chat";
            }

            public static string PostBlock(string deterredUserId)
            {
                return $"api/v1/Chat/block/{deterredUserId}";
            }

            public static string PostUnBlock(string deterredUserId)
            {
                return $"api/v1/Chat/unblock/{deterredUserId}";
            }

            public static string PostReadMessages(string conversationId)
            {
                return $"api/v1/Chat/{conversationId}/ReadMessages";
            }

            public static string GetBlockedStatus(string deterredUserId)
            {
                return $"api/v1/Chat/Block/Status/{deterredUserId}";
            }

            public static string GetUserBlockedList(bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Chat/block" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Chat/block";
            }

            public static string GetUserMessages(bool paginated = false, int pageSize = 4, int pageNumber = 1)
            {
                return paginated
                    ? $"api/v1/Chat" + Paginated(pageSize, pageNumber)
                    : $"api/v1/Chat";
            }

            public static string GetUnReadMessageCount()
            {
                return $"api/v1/Chat/UnReadMessageCount";
            }

            public static string DeleteMessages(string conversationId)
            {
                return $"api/v1/Chat/{conversationId}";
            }

            public static string Paginated(int pageSize, int pageNumber)
            {
                return new PagingModel
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                }.ToString();
            }
        }
    }
}