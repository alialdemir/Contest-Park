using ContestPark.Core.FunctionalTests;
using ContestPark.Core.Services.Identity;
using ContestPark.Duel.API.Services.Follow;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Duel.API.FunctionalTests
{
    public class DuelTestStartup : Startup
    {
        public DuelTestStartup(IConfiguration env) : base(env)
        {
        }

        protected override void ConfigureOtherService(IServiceCollection services)
        {
            services.AddSingleton<IFollowService, FollowMockService>();
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
