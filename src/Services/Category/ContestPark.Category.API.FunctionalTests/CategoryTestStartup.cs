using ContestPark.Core.FunctionalTests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace ContestPark.Category.API.FunctionalTests
{
    public class CategoryTestStartup : Startup
    {
        public CategoryTestStartup(IConfiguration env) : base(env)
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