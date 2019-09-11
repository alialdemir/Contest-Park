using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Reflection;

namespace ContestPark.Core.Dapper.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost, Action<IServiceProvider, Action<string, Assembly[], Action>> seeder)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<IWebHost>>();

                var configuration = services.GetRequiredService<IConfiguration>();
                if (configuration["IsMigrateDatabase"] == null || configuration["IsMigrateDatabase"] == "false")
                    return webHost;

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(WebHostExtensions).Name}");

                    var retry = GetRetryPolicy();

                    retry.Execute(() =>
                    {
                        //if the sql server container is not created on run docker compose this
                        //migration can't fail for network related exception. The retry options for DbContext only
                        //apply to transient exceptions.

                        seeder(services,
                      (connectionString, assemblies, databaseSeeder) =>
                      {
                          UpdateDatabaseAsync(services, connectionString, databaseSeeder, assemblies);
                      });
                    });

                    logger.LogInformation($"Migrated database associated with context {typeof(WebHostExtensions).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(WebHostExtensions).Name}");
                }
            }

            return webHost;
        }

        /// <summary>
        /// Update the database
        /// </sumamry>
        private static void UpdateDatabaseAsync(IServiceProvider services, string connectionString, Action databaseSeeder, params Assembly[] assemblies)
        {
            var retry = GetRetryPolicy();

            var logger = services.GetRequiredService<ILogger<IWebHost>>();

            try
            {
                retry.Execute(async () =>
                {
                    logger.LogInformation($"Migrating database associated with context {nameof(UpdateDatabaseAsync)}");

                    // Instantiate the runner
                    IServiceProvider serviceProvider = CreateServices(connectionString, assemblies);
                    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                    bool isCreatedDatabase = await runner.CreateDatabaseIfNotExistsAsync(connectionString);
                    if (isCreatedDatabase)
                    {
                        runner.MigrateUp();
                        databaseSeeder();
                    }

                    logger.LogInformation($"Migrated database associated with context {nameof(UpdateDatabaseAsync)}");
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating the database used on context {nameof(UpdateDatabaseAsync)}");
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </sumamry>
        private static IServiceProvider CreateServices(string connectionString, params Assembly[] assemblies)
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    //.AddSQLite()
                    .AddMySql5()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations

                    .ScanIn(assemblies).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
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
