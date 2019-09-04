using Amazon.CloudWatchLogs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;
using System;
using System.IO;

namespace ContestPark.Signalr.API
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
