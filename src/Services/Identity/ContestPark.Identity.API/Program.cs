using Amazon.CloudWatchLogs;
using ContestPark.EventBus.IntegrationEventLogEF;
using ContestPark.Identity.API.Data;
using ContestPark.Identity.API.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;
using System;
using System.IO;

namespace ContestPark.Identity.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .UseSerilog()
                .Build();

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);

                var host = BuildWebHost(configuration, args);

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);

                host
                    .MigrateDatabase<PersistedGrantDbContext>((_, __) => { })
                    .MigrateDatabase<ApplicationDbContext>((context, services) =>
                    {
                        var env = services.GetService<IHostingEnvironment>();
                        var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
                        var settings = services.GetService<IOptions<IdentitySettings>>();

                        new ApplicationDbContextSeed()
                            .SeedAsync(context, env, logger, settings)
                            .Wait();
                    })
                    .MigrateDatabase<ConfigurationDbContext>((context, services) =>
                    {
                        new ConfigurationDbContextSeed()
                            .SeedAsync(context)
                            .Wait();
                    })
                    .MigrateDatabase<IntegrationEventLogContext>((_, __) => { });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration);

            #region ClouldWatch settings

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Production)// Sadece prod ortamında clouldwatch'a yazıyoruz
            {
                var cloudWatchLogsClient = new AmazonCloudWatchLogsClient(configuration["AwsAccessKeyId"], configuration["AwsSecretAccessKey"], Amazon.RegionEndpoint.EUCentral1);
                loggerConfiguration.WriteTo.AmazonCloudWatch(new CloudWatchSinkOptions
                {
                    LogGroupName = configuration["AwsLogGroupName"],
                    LogStreamNameProvider = new ConstantLogStreamNameProvider(AppName),
                    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
                    TextFormatter = new Serilog.Formatting.Json.JsonFormatter(),
                }, cloudWatchLogsClient);
            }
            else
            {
                loggerConfiguration.WriteTo.Console();
            }

            #endregion ClouldWatch settings

            return loggerConfiguration.CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
