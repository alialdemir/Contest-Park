using ContestPark.EventBus.IntegrationEventLogEF;
using ContestPark.Identity.API.Data;
using ContestPark.Identity.API.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Polly;
using Polly.Retry;
using System;
using System.Linq;

namespace ContestPark.Identity.API.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDatabase<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    var retry = GetRetryPolicy();

                    retry.Execute(() =>
                   {
                       //if the sql server container is not created on run docker compose this
                       //migration can't fail for network related exception. The retry options for DbContext only
                       //apply to transient exceptions.
                       try
                       {
                           /*
                                  Her db context için eğer tablolar oluşmamışsa exception fırlatır migrate çalışır sonra tekrar kontrol eder
                           */

                           if (context is ApplicationDbContext)
                           {
                               (context as IdentityDbContext<ApplicationUser>).Users.Any();
                           }
                           else if (context is PersistedGrantDbContext)
                           {
                               (context as PersistedGrantDbContext).PersistedGrants.Any();
                           }
                           else if (context is ConfigurationDbContext)
                           {
                               (context as ConfigurationDbContext).Clients.Any();
                           }
                           else if (context is IntegrationEventLogContext)
                           {
                               (context as IntegrationEventLogContext).IntegrationEventLogs.Any();
                           }

                           seeder(context, services);
                       }
                       catch (MySqlException)
                       {
                           // entity framework kendi migrations scriptlerini çalıştırınca auto increment özelliğini aktif edemiyor. Sanırım bug var
                           // o yüzden manuel çalıştırdım

                           bool isEnsureCreated = context.Database.EnsureCreated();
                           if (!isEnsureCreated)
                           {
                               string dbScript = context.Database.GenerateCreateScript();

                               context.Database.ExecuteSqlCommand(new RawSqlString(dbScript));
                           }

                           MigrateDatabase(webHost, seeder);
                       }
                   });

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
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