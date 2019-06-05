using ContestPark.Core.CosmosDb;
using ContestPark.Core.CosmosDb.Interfaces;
using ContestPark.Core.CosmosDb.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class Startup
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton((_) =>
            {
                return new DocumentDbConnection
                {
                    CosmosDbAuthKeyOrResourceToken = configuration["CosmosDbAuthKeyOrResourceToken"],
                    CosmosDbServiceEndpoint = configuration["CosmosDbServiceEndpoint"],
                    CosmosDbDatabaseId = configuration["CosmosDbDatabaseId"]
                };
            });

            services.AddSingleton<IDocumentDbInitializer, DocumentDbInitializer>();
            services.AddSingleton(typeof(IDocumentDbRepository<>), typeof(DocumentDbRepository<>));

            return services;
        }
    }
}