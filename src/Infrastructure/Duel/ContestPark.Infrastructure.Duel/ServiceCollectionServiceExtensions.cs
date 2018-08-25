using ContestPark.Infrastructure.Duel.Repositories.Duel;
using ContestPark.Infrastructure.Duel.Repositories.DuelInfo;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Infrastructure.Duel
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddDuelRegisterService(this IServiceCollection services)
        {
            services.AddTransient<IDuelRepository, DuelRepository>();
            services.AddTransient<IDuelInfoRepository, DuelInfoRepository>();

            return services;
        }
    }
}