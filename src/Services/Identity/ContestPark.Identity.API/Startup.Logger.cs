using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace ContestPark.Identity.API
{
    public partial class Startup
    {
        private void LoggerConfigure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //loggerFactory.AddAzureWebAppDiagnostics();
            //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);
        }
    }
}