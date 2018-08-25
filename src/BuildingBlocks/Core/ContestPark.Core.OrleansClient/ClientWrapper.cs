using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Core.OrleansClient
{
    public static class ClientWrapper
    {
        public static IServiceCollection UseOrleansClient(this IServiceCollection services,
            IConfiguration configuration)
        {
            var clientBuilderContext = new ClientBuilderContext
            {
                Configuration = configuration,
            };

            return services.UseOrleansClient(clientBuilderContext);
        }

        /// <summary>
        /// Orleans client için gerekli startup
        /// </summary>
        /// <param name="configuration">Connection string almak için configuration</param>
        /// <returns></returns>
        public static IServiceCollection UseOrleansClient(this IServiceCollection services, ClientBuilderContext clientBuilderContext)
        {
            if (clientBuilderContext == null)
                throw new ArgumentNullException(nameof(clientBuilderContext));

            IClusterClient client = new ClientBuilder()
                                                    .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                                                    .UseAdoNetClustering(x =>
                                                    {
                                                        x.ConnectionString = clientBuilderContext.ConnectionString;
                                                        x.Invariant = "System.Data.SqlClient";
                                                    })
                                                    .UseConfiguration(clientBuilderContext)
                                                    .UseSignalR()
                                                    .Build();

            client.Connect(RetryFilter).GetAwaiter().GetResult();

            services.AddSingleton(client);

            return services;
        }

        private static ClientConfiguration GeetClientConfiguration(string siloHostIp, int siloHostPort)
        {
            var clientConfig = ClientConfiguration.LocalhostSilo();
            clientConfig.Gateways.Add(new IPEndPoint(IPAddress.Parse(siloHostIp), siloHostPort));

            return clientConfig;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return true;
        }

        public static IClientBuilder UseConfiguration(
            this IClientBuilder clientBuilder,
            ClientBuilderContext context
        )
        {
            return clientBuilder
                        .UseConfiguration(GeetClientConfiguration(context.SiloHostIp, context.SiloHostPort))
                         .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "ContestParkClusrerId";
                            options.ServiceId = "ContestParkServiceId";
                        });
        }
    }
}