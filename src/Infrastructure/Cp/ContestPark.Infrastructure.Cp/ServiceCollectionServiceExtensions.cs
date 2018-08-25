using ContestPark.Infrastructure.Cp.Repositories.Cp;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Infrastructure.Cp
{
    public static partial class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddCpRegisterService(this IServiceCollection services)
        {
            services.AddTransient<ICpRepository, CpRepository>();

            return services;
        }
    }
}