using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;

namespace ContestPark.Core.Database.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDatabase<TContext>(this IWebHost webHost, Action<IServiceProvider, ILogger<TContext>> seeder)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                var retry = GetRetryPolicy();

                retry.Execute(() =>
                {
                    try
                    {
                        seeder(services, logger);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                    }
                });

                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }

            return webHost;
        }

        private static RetryPolicy GetRetryPolicy()
        {
            return Policy.Handle<Exception>()
                .WaitAndRetry(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(30),
                });
        }
    }
}
