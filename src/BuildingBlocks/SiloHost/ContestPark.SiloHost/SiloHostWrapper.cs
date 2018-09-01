using ContestPark.Core.DataSeed;
using ContestPark.Core.Interfaces;
using ContestPark.Core.Model;
using ContestPark.Infrastructure.Category;
using ContestPark.Infrastructure.Cp;
using ContestPark.Infrastructure.Duel;
using ContestPark.Infrastructure.Identity;
using ContestPark.Infrastructure.Question;
using ContestPark.Infrastructure.Signalr;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace ContestPark.SiloHost
{
    public class SiloHostWrapper
    {
        public static SiloHostWrapper Instance { get; set; } = new SiloHostWrapper();
        private ISiloHost silo;
        private readonly ManualResetEvent siloStopped = new ManualResetEvent(false);

        public void Init()
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            string invariant = "System.Data.SqlClient";

            silo = new SiloHostBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "ContestParkClusrerId";
                    options.ServiceId = "ContestParkServiceId";
                })
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                })
                .UseAdoNetReminderService(options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                })
                .AddAdoNetGrainStorage("GrainStorage", options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                })
                .AddMemoryGrainStorage("PubSubStore")
                .ConfigureServices(svc =>
                {
                    svc.AddSingleton<ISettingsBase, SettingsBase>();

                    svc.AddCategoryRegisterService();

                    svc.AddCpRegisterService();

                    svc.AddDuelRegisterService();

                    svc.AddIdentityRegisterService();

                    svc.AddQuestionRegisterService();

                    svc.AddSignalrRegisterService();
                })
                .AddStartupTask(async (services, cancellationToken) =>
                {
                    var logger = services.GetService<ILogger<DatabaseMigration>>();
                    await DatabaseMigration.MigrateUp(logger);
                })
                .UseSignalR()
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                // .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning).AddConsole())
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(StopSilo);
                siloStopped.WaitOne();
            };

            siloStopped.WaitOne();
        }

        private async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo started");
        }

        private async Task StopSilo()
        {
            await silo.StopAsync();
            Console.WriteLine("Silo stopped");
            siloStopped.Set();
        }
    }
}