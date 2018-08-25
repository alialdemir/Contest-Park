using ContestPark.Infrastructure.Identity.Repositories.User;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Infrastructure.Identity
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddIdentityRegisterService(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}