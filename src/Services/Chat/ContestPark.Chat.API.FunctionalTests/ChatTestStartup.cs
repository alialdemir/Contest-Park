using ContestPark.Core.FunctionalTests;
using ContestPark.Core.Services.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestPark.Chat.API.FunctionalTests
{
    public class ChatTestStartup : Startup
    {
        public ChatTestStartup(IConfiguration env) : base(env)
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
