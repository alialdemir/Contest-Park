using ContestPark.Core.FunctionalTests;
using ContestPark.Core.Services.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class FollowTestStartup : Startup
    {
        public FollowTestStartup(IConfiguration env) : base(env)
        {
        }

        protected override void AddSingleton(IServiceCollection services)
        {
            services.AddSingleton<IIdentityService, IdentityMockService>();
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
