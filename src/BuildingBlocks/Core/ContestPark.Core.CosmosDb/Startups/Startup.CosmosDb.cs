using ContestPark.Core.CosmosDb;
using ContestPark.Core.Database.Interfaces;
using Cosmonaut;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class Startup
    {
        private static CosmosStoreSettings cosmosStoreSettings = null;

        public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton((_) =>
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    Converters = { new StringEnumConverter() }
                };

                return new CosmosStoreSettings(configuration["CosmosDbDatabaseId"],
                                               configuration["CosmosDbServiceEndpoint"],
                                               configuration["CosmosDbAuthKeyOrResourceToken"])
                {
                    JsonSerializerSettings = serializerSettings,
                    ConnectionPolicy = new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Https,
                    }
                };
            });

            services.AddSingleton(typeof(IRepository<>), typeof(DocumentDbRepository<>));

            services.AddSingleton(typeof(ICosmosStore<>), typeof(CosmosStore<>));

            return services;
        }
    }
}
