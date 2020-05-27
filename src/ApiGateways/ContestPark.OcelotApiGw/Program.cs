using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace ContestPark.OcelotApiGw
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if DEBUG
                                .ConfigureAppConfiguration(ic => ic.AddJsonFile("configuration.json"))
#else
                                .ConfigureAppConfiguration(ic => ic.AddJsonFile("configuration.production.json"))
#endif
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

            return loggerConfiguration.CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            return builder.Build();
        }

        //        public static void Main(string[] args) => BuildWebHost(args).Run();

        //        public static IWebHost BuildWebHost(string[] args)
        //        {
        //            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
        //            builder.ConfigureServices(s => s.AddSingleton(builder))
        //#if DEBUG
        //                .ConfigureAppConfiguration(ic => ic.AddJsonFile("configuration.json"))
        //#else
        //                .ConfigureAppConfiguration(ic => ic.AddJsonFile("configuration.production.json"))
        //#endif
        //                .UseStartup<Startup>()
        //                .ConfigureLogging((hostingContext, loggingbuilder) =>
        //                {
        //                    loggingbuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        //                    loggingbuilder.AddConsole();
        //                    loggingbuilder.AddDebug();
        //                })
        //                .UseSerilog((builderContext, config) =>
        //                {
        //                    config
        //                        .MinimumLevel.Information()
        //                        .Enrich.FromLogContext()
        //                        .WriteTo.Console();
        //                });
        //            IWebHost host = builder.Build();

        //            return host;
        //        }
    }
}
