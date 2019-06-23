using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ContestPark.Chat.API.FunctionalTests
{
    public class ChatTestStartup : Startup
    {
        public ChatTestStartup(IConfiguration env) : base(env)
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