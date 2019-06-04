using ContestPark.EventBus.IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class Startup
    {
        public static IServiceCollection AddIntegrationEventLogEFDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseMySql(connectionString,
                                     mySqlOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }
    }
}