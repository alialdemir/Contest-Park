using ContestPark.Infrastructure.Cp.Repositories.Cp;
using ContestPark.Infrastructure.Cp.Repositories.CpInfo;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Infrastructure.Cp
{
    public static partial class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddCpRegisterService(this IServiceCollection services)
        {
            services.AddTransient<ICpRepository, CpRepository>();

            services.AddTransient<ICpInfoRepository, CpInfoRepository>();

            return services;
        }
    }
}