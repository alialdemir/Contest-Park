using ContestPark.Core.Dapper;
using ContestPark.Core.Dapper.Abctract;
using ContestPark.Core.Dapper.Interfaces;
using ContestPark.Core.Database.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class Startup
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDatabaseConnection, DatabaseConnection>();

            services.AddSingleton(typeof(IRepository<>), typeof(DapperRepository<>));

            return services;
        }
    }
}
