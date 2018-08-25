using ContestPark.Infrastructure.Category.Repositories.Category;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Infrastructure.Category
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddCategoryRegisterService(this IServiceCollection services)
        {
            services.AddTransient<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}