using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContestPark.Core.Service.Startup
{
    public partial class Startup
    {
        public static IApplicationBuilder LoggerConfigure(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            //if (env.IsDevelopment())
            //{
            loggerFactory
                .AddConsole(configuration.GetSection("Logging"))
                .AddDebug();
            //}

            //if (env.IsProduction())
            //{
            loggerFactory.AddAzureWebAppDiagnostics()
                .AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
            //}
            return app;
        }
    }
}