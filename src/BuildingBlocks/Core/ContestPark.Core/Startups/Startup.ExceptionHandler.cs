using ContestPark.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExceptionHandlerStartup
    {
        /// <summary>
        /// Servislerin döndürdüğü exception mesajlarını startdartlaştırır
        /// </summary>
        public static IApplicationBuilder UseExceptionHandlerConfigure(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(
        builder =>
        {
            builder.Run(
            async context =>
            {
                ILoggerFactory loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                IExceptionHandlerFeature exception = context.Features.Get<IExceptionHandlerFeature>();
                if (exception != null && loggerFactory != null)
                {
                    var serviceName = Assembly.GetEntryAssembly().GetName().Name.Replace("ContestPark.", "");
                    loggerFactory
                    .CreateLogger(serviceName)
                    .LogError($"{exception.GetType().Name} - Message:{exception.Error.Message} - StackTrace:{exception.Error.StackTrace}");

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    await context.Response.WriteAsync(new ErrorDetailsModel()
                    {
                        StatusCode = context.Response.StatusCode,
#if DEBUG
                        Message = exception.Error.Message + "   " + exception.Error.StackTrace
#else
                        Message = "Internal Server Error."
#endif
                    }.ToString());
                }
            });
        });

            return app;
        }
    }
}