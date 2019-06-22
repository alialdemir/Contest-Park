using ContestPark.Category.API.Services.Balance;
using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Category.API.FunctionalTests
{
    public class CategoryTestStartup : Startup
    {
        public CategoryTestStartup(IConfiguration env) : base(env)
        {
        }

        protected override void AddTransient(IServiceCollection services)
        {
            services.AddSingleton<IBalanceService, BalanceServiceMock>();
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
        }
    }
}