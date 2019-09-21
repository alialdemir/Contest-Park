using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthStartup
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var identityUrl = configuration.GetValue<string>("IdentityUrl");
                var audience = configuration.GetValue<string>("Audience");

                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = audience;
            });

            return services;
        }

        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            return app;
        }
    }
}
