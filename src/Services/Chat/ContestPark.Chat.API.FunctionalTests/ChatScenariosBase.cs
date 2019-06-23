﻿using ContestPark.Chat.API.Infrastructure;
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