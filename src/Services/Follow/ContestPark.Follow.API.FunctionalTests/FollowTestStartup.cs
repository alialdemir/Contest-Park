using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ContestPark.Follow.API.FunctionalTests
{
    public class FollowTestStartup : Startup
    {
        public FollowTestStartup(IConfiguration env) : base(env)
        {
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