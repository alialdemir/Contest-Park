using ContestPark.Chat.API.FunctionalTests;
using ContestPark.Core.FunctionalTests;
using ContestPark.Core.Services.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Post.API.FunctionalTests
{
    public class PostTestStartup : Startup
    {
        public PostTestStartup(IConfiguration env) : base(env)
        {
        }

        protected override void ConfigureIdentityService(IServiceCollection services)
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
