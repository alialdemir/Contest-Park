using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ContestPark.Balance.API.FunctionalTests
{
    public class BalanceTestStartup : Startup
    {
        public BalanceTestStartup(IConfiguration env) : base(env)
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