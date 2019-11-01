using ContestPark.Core.Dapper;
using ContestPark.Core.Dapper.Abctract;
using ContestPark.Core.Dapper.Interfaces;
using ContestPark.Core.Database.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class Startup
    {
        public static IServiceCollection AddMySql(this IServiceCollection services)
        {
            services.AddTransient<IDatabaseConnection, DatabaseConnection>();

            services.AddSingleton(typeof(IQueryRepository), typeof(DapperQueryRepository));
            services.AddSingleton(typeof(IRepository<>), typeof(DapperRepository<>));

            return services;
        }
    }
}
