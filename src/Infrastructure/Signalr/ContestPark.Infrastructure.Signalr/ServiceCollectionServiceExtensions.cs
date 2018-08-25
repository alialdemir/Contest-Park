using ContestPark.Infrastructure.Signalr.Repositories.DuelUser;
using ContestPark.Infrastructure.Signalr.Services.Signalr;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace ContestPark.Infrastructure.Signalr
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddSignalrRegisterService(this IServiceCollection services)
        {
            services.AddSingleton<IDuelUserRepository, DuelUserRepository>();

            //services.AddSingleton<ISignalrService, SignalrService>();

            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                string redisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString");

                var configuration = ConfigurationOptions.Parse(redisConnectionString, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });

            return services;
        }
    }
}