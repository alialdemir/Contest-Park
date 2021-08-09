using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Core.Startups
{
    public static class Startup
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen();

            return services;
        }

        public static IApplicationBuilder UseSwaggerGen(this IApplicationBuilder app, string serviceName)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", serviceName));

            return app;
        }
    }
}